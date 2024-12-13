using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SG
{
    
    public class AnimationClipTest : MonoBehaviour
    {   public ActorController actorController;
        public Animator animator;
        public AnimationClip clip;
        
        public UnityAction Action;
    

        private void Start()
        {
            
        }

        private void Update()
        {   AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            AnimatorClipInfo[] nextClipInfo = animator.GetNextAnimatorClipInfo(0);
            foreach (var v in currentClipInfo)
            {   float totalFrames = v.clip.frameRate * v.clip.length;
                int currentFrame = Mathf.FloorToInt(stateInfo.normalizedTime % 1 * totalFrames);
            //    Debug.Log("current "+v.clip.name+" "+v.clip.length+" "+stateInfo.normalizedTime+" "+stateInfo.IsName("W_AttackRightLight1"));
            //    Debug.Log(currentClipInfo.Length);
            }

            foreach (var v in nextClipInfo)
            {
             //  Debug.Log("next "+v.clip.name+" "+v.clip.length);
            }
            
        }

      
    }
    
    
    
}