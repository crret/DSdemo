using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG
{


    public class ApplyRM : StateMachineBehaviour
    {    [SerializeField] private int TransitionFlag = 0;

        private PlayerAttacker playerAttacker;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerAttacker = animator.GetComponentInParent<PlayerAttacker>();
            animator.SetBool("isAnimPlaying", true);
            animator.SetBool("isInteracting", true);
            playerAttacker.canDoCombo = false;
         
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        [SerializeField]
        private float brokeInteractingTime = 0.7f;
        [SerializeField]
        private float brokeAttackStiffTime = 0.3f;
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {  
           // Debug.Log(stateInfo.normalizedTime + ", " + stateInfo.shortNameHash+", " + GetInstanceID());
            
            if (stateInfo.normalizedTime > brokeInteractingTime)
            {animator.SetBool("isInteracting", false);}
            else
            {
                animator.SetBool("isInteracting", true);
            }
           
            if(stateInfo.normalizedTime > brokeAttackStiffTime&&playerAttacker.canDoCombo ==false&&playerAttacker.heavyAttackSp==false)
            {   // Debug.Log(stateInfo.normalizedTime + ", " + stateInfo.shortNameHash);
                
                playerAttacker.canDoCombo = true;
                
            }
           
            animator.SetBool("isAnimPlaying", true);
            
             
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("isAnimPlaying", false);
            animator.SetBool("isInteracting", false);
            animator.SetBool("isTurnableAnimPlaying",false);    
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()

        /*
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that processes and affects root motion
            // animator.ApplyBuiltinRootMotion();
        }
        */

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}