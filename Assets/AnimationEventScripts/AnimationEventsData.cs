using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SG
{   
    public enum ClipEventType
    {
        
        SetIK=0,
        
        SetHeavySp=10,
    }
    
    public enum ClipEventCellType
    {
        
        SetRightIK=0,
        SetLeftIK=1,
        
        SetHeavySp=10,
    }
    public enum SetIKType
    {
        SetRightIKOff=0,
        SetLeftIKOff=1,
    }
    public enum SetHeavySpType
    {
        SetHeavySp=0,
        
        OpenRightDamageCollider=10,
    }
    [CreateAssetMenu(menuName = "AnimtionEventsData")]
    public class AnimationEventsData : ScriptableObject
    {   
        public AnimationClip animationClip;
        public int StartFrame = 0;
        public int EndFrame = 0;
        private Func<AnimationEventsData,IEnumerator>  AnimFunc;
        public bool stopCoroutineIfAnimExit=true;
        public bool isCoroutineRunning = false;
        public IEnumerator EventCoroutine;
        [FormerlySerializedAs("animEventType")] public ClipEventType clipEventType;
  
        public string GetTypeName()
        {
            
            if (this is SetIKEvent tmpSetIKEvent)
            {
                return tmpSetIKEvent.setIKType.ToString();
            }
            else if (this is SetHeavySpEvent tmpSetHeavySpEvent)
            {
                return tmpSetHeavySpEvent.setHeavySpType.ToString();
            }

            return "fail";
        }

       

        public void SetBaseFunc(Func<AnimationEventsData, IEnumerator> animFunc)
        {
             
            AnimFunc = animFunc;
            
        }

        public virtual void SetFunc<T>(Func<T, IEnumerator> animFunc) where T:AnimationEventsData
        {
            
        }

    }

}