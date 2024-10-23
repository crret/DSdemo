using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity)
        {
        }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }

        public bool ContainsKey(string name)
        {
            return this.Exists(x => x.Key.name.Equals(name));
        }

        public bool ContainsKeyEnd(string suffix)
        {
            return this.Exists(x => x.Key.name.EndsWith(suffix));
        }

        public void OverrideClips(WeaponItem weaponItem, bool isLeft, bool isTwoHand)
        {
            // 根据是否是双手武器，选择合适的动画片段
            var animationMap = new Dictionary<string, string>();

            if (isTwoHand)
            {
                animationMap["00000"] = weaponItem.Two_Hand_Idle;
                animationMap["20100"] = weaponItem.Two_Hand_Move;
                animationMap["20200"] = weaponItem.Two_Hand_Run;

            }
            else
            {
                animationMap["00000"] = weaponItem.Right_Hand_Idle;
                animationMap["20100"] = weaponItem.Right_Hand_Move;
                animationMap["20200"] = weaponItem.Right_Hand_Run;
            }

            if (weaponItem.isUnarmed == false)
            {
                animationMap["30000"] = weaponItem.RH_Light_Attack_01;
                animationMap["30010"] = weaponItem.RH_Light_Attack_02;
                animationMap["30020"] = weaponItem.RH_Light_Attack_03;
            
                animationMap["30320"] = weaponItem.RH_HeavyStart_Attack_01;
                animationMap["30321"] = weaponItem.RH_HeavyEnd_Attack_01;
                animationMap["30330"] = weaponItem.RH_HeavySubStart_Attack;
                animationMap["30340"] = weaponItem.RH_HeavyStart_Attack_02;
                animationMap["30341"] = weaponItem.RH_HeavyEnd_Attack_02;
                animationMap["30500"] = weaponItem.RH_LightDash;
                animationMap["30900"] = weaponItem.RH_LightStep;
                animationMap["34000"] = weaponItem.TH_Light_Attack_01;
                animationMap["34010"] = weaponItem.TH_Light_Attack_02;
                animationMap["34020"] = weaponItem.TH_Light_Attack_03;
            
                animationMap["34320"] = weaponItem.TH_HeavyStart_Attack_01;
                animationMap["34321"] = weaponItem.TH_HeavyEnd_Attack_01;
                animationMap["34330"] = weaponItem.TH_HeavySubStart_Attack;
                animationMap["34340"] = weaponItem.TH_HeavyStart_Attack_02;
                animationMap["34341"] = weaponItem.TH_HeavyEnd_Attack_02;
            }
            
        
            // 遍历所有动画片段
            for (int i = 0; i < this.Count; i++)
            {
                var clip = this[i];

                // 判断是否需要替换的动画片段
                foreach (var suffix in animationMap.Keys)
                {
                    if (clip.Key.name.EndsWith(suffix))
                    {
                        // 替换对应的动画片段
                        this[clip.Key.name] = LoadAnimationsFromFolder.AnimationClipsDict[animationMap[suffix]];
                        Debug.Log("Loaded and replaced AnimationClip: " + clip.Key.name);
                    }
                }
            }
        }
        public void OverrideTransitionClips(WeaponItem weaponItem, bool isLeft, bool isTwoHand)
        {
            // 定义一个字典，用于存储不同情况下的动画片段映射
            var animationMap = new Dictionary<string, string>();

            if (isTwoHand)
            {
                animationMap["00000Transition"] = weaponItem.Two_Hand_Idle + "Transition";
                animationMap["20100Transition"] = weaponItem.Two_Hand_Move + "Transition";
                animationMap["20200Transition"] = weaponItem.Two_Hand_Run + "Transition";
            }
            else if(isLeft==false)
            {
                animationMap["00000Transition"] = weaponItem.Right_Hand_Idle + "Transition";
                animationMap["20100Transition"] = weaponItem.Right_Hand_Move + "Transition";
                animationMap["20200Transition"] = weaponItem.Right_Hand_Run + "Transition";
            }

            // 遍历所有动画片段
            for (int i = 0; i < this.Count; i++)
            {
                var clip = this[i];

                // 检查动画片段的名称是否与字典中的后缀匹配
                foreach (var suffix in animationMap.Keys)
                {
                    if (clip.Key.name.EndsWith(suffix))
                    {
                        // 替换对应的动画片段
                        this[clip.Key.name] = LoadAnimationsFromFolder.AnimationClipsDict[animationMap[suffix]];
                        Debug.Log("Loaded and replaced AnimationClip: " + clip.Key.name);
                    }
                }
            }
        }
    }
}