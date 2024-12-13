using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class RightLightAttack03 : StateMachineBehaviour
    {
        private PlayerAttacker playerAttacker;
        [SerializeField] private int TransitionFlag = 0;

        [SerializeField] private bool SetCancelRMOnTransitionStart = false;

        [SerializeField] private bool SetCancelRMOnTransitionEnd = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerAttacker = animator.GetComponentInParent<PlayerAttacker>();
            playerAttacker.inRightLightAttack03 = true;
            TransitionFlag = 0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.IsInTransition(0) == false && TransitionFlag == 0)
            {
                TransitionFlag++;
                animator.SetBool("CancelRMInTransition", false);
            } //TransitionFlag为0时，表示动画在进入这段过渡，1则为结束时的过渡

            if (animator.IsInTransition(0) && TransitionFlag == 0)
            {
                if (SetCancelRMOnTransitionStart)
                {
                    animator.SetBool("CancelRMInTransition", true);
                }
            }

            if (animator.IsInTransition(0) && TransitionFlag == 1)
            {
                if (SetCancelRMOnTransitionEnd)
                {
                    animator.SetBool("CancelRMInTransition", true);
                }

                playerAttacker.inRightLightAttack03 = false;
            }
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}