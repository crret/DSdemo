using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Events;
using System.Linq;
namespace SG
{

    public class InteractBoolList : MonoBehaviour
    {

        public List<Func<bool>> AnimForbidBoolList = new List<Func<bool>>();
        public List<Func<bool>> ComboForbidTriggerList = new List<Func<bool>>();
        public List<Func<bool>> InAttack = new List<Func<bool>>();
        public PlayerManager playerManager;
        public ActorController actorController;
        public PlayerAttacker playerAttacker;
        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            actorController= GetComponentInChildren<ActorController>();
            playerAttacker=GetComponent<PlayerAttacker>();
        }

        private void Start()
        {
            AnimForbidBoolList.Add(() => playerManager.isInteracting);
            AnimForbidBoolList.Add(() => playerManager.isChangeWeapon);
            ComboForbidTriggerList.Add(() => !playerAttacker.canDoCombo);
            
            InAttack.Add(() => playerAttacker.inRightLightAttack01);
            InAttack.Add(() => playerAttacker.inRightLightAttack02);
            InAttack.Add(() => playerAttacker.inRightLightAttack03);
            InAttack.Add(()=>playerAttacker.inBothLightAttack01);
            InAttack.Add(()=>playerAttacker.inBothLightAttack02);
            InAttack.Add(()=>playerAttacker.inBothLightAttack03);
            InAttack.Add(()=>playerAttacker.inRightHeavy01Start);
            InAttack.Add(()=>playerAttacker.inRightHeavy01End);
            InAttack.Add(()=>playerAttacker.inRightHeavy02Start);
            InAttack.Add(()=>playerAttacker.inRightHeavy02End);
            InAttack.Add(()=>playerAttacker.inRightLightDash);
            InAttack.Add(()=>playerAttacker.inBothLightDash);
            InAttack.Add(()=>playerAttacker.inBothHeavy01SubStart);
            InAttack.Add(()=>playerAttacker.inBothHeavy01Start);
            InAttack.Add(()=>playerAttacker.inBothHeavy01End);
            InAttack.Add(()=>playerAttacker.inBothHeavy02Start);
            InAttack.Add(()=>playerAttacker.inBothHeavy02End);
            InAttack.Add(()=>playerAttacker.inRightLightDash);
            InAttack.Add(()=>playerAttacker.inRightLightStep);
        }

        private void Update()
        {

        }

        public void AddAnimForbidBool(Func<bool> boolFunc)
        {
            AnimForbidBoolList.Add(boolFunc);
        }

        public bool CanAnimPlay()
        {
            foreach (var boolFunc in AnimForbidBoolList)
            {
                if (boolFunc()) // 动态调用布尔变量的值
                {
                    return false; // 只要有一个为true，返回 false
                }
            }

            return true;
        }

        public bool CanComboPlay()
        {
           
                foreach (var boolFunc in ComboForbidTriggerList)
                {
                    if (boolFunc()) // 动态调用布尔变量的值
                    {
                        return false; // 只要有一个为true，返回 false
                    }
                }
                return true;
          
          
        }
    }
}