using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{    public enum StateBool
    {
      
        inRightLightAttack01=1,
        inRightLightAttack02=2,
        inRightLightAttack03=3,
        inRightLightDash=4,
        inRightLightStep=5,
        inBothLightAttack01=10,
        inBothLightAttack02=11,
        inBothLightAttack03=12,
        inBothLightDash=13,
        inBothLightStep=14,
        inRightHeavy01SubStart=20,
        inRightHeavy01Start=21,
        inRightHeavy01End=22,
        inRightHeavy02Start=23,
        inRightHeavy02End=24,
        inBothHeavy01SubStart=30,
        inBothHeavy01Start=31,
        inBothHeavy01End=32,
        inBothHeavy02Start=33,
        inBothHeavy02End=34,
        inLeftLightAttack01=40,
        inLeftLightAttack02=41,
        lockTurn=99,
        rolling=100,
        backStep=101,
            // 这里可以加入 playerAttacker 中所有的 bool 变量
    }

    public class CheckState : StateMachineBehaviour
    {
        private PlayerAttacker playerAttacker;
        private ActorController actorController;
        [SerializeField] private int TransitionFlag = 0;

        [SerializeField] private bool SetCancelRMOnTransitionStart = false;

        [SerializeField] private bool SetCancelRMOnTransitionEnd = false;
        [SerializeField] private StateBool state;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerAttacker = animator.GetComponentInParent<PlayerAttacker>();
            actorController = animator.GetComponent<ActorController>();
            SetBoolVariable(true);
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
            
                SetBoolVariable(false);
            }
        }
        
        private void SetBoolVariable(bool value)
        {
            switch (state)
            { 
                
                case StateBool.inRightLightAttack01:
                    playerAttacker.inRightLightAttack01 = value;
                    break;
                case StateBool.inRightLightAttack02:
                    playerAttacker.inRightLightAttack02 = value;
                    break;
                case StateBool.inRightLightAttack03:
                    playerAttacker.inRightLightAttack03 = value;
                    break;
                case StateBool.inRightLightDash:
                    playerAttacker.inRightLightDash = value;
                    break;
                case StateBool.inRightLightStep:
                    playerAttacker.inRightLightStep = value;
                    break;
                case StateBool.inBothLightAttack01:
                    playerAttacker.inBothLightAttack01 = value;
                    break;
                case StateBool.inBothLightAttack02:
                    playerAttacker.inBothLightAttack02 = value;
                    break;
                case StateBool.inBothLightAttack03:
                    playerAttacker.inBothLightAttack03=value;
                    break;
                case StateBool.inBothLightDash:
                    playerAttacker.inBothLightDash = value;
                    break;
                case StateBool.inBothLightStep:
                    playerAttacker.inBothLightStep = value;
                    break;
                case StateBool.inRightHeavy01Start:
                    playerAttacker.inRightHeavy01Start = value;
                    break;
                case StateBool.inRightHeavy02Start:
                    playerAttacker.inRightHeavy02Start = value;
                    break;
                case StateBool.inRightHeavy01SubStart:
                    playerAttacker.inRightHeavy01SubStart = value;
                    break;
                case StateBool.inRightHeavy01End:
                    playerAttacker.inRightHeavy01End = value;
                    break;
                case StateBool.inRightHeavy02End:
                    playerAttacker.inRightHeavy02End = value;
                    break;
                case StateBool.inBothHeavy01SubStart:
                    playerAttacker.inBothHeavy01SubStart = value;
                    break;
                case StateBool.inBothHeavy01Start:
                    playerAttacker.inBothHeavy01Start = value;
                    break;
                case StateBool.inBothHeavy01End:
                    playerAttacker.inBothHeavy01End = value;
                    break;
                case StateBool.inBothHeavy02Start:
                    playerAttacker.inBothHeavy02Start = value;
                    break;
                case StateBool.inBothHeavy02End:
                    playerAttacker.inBothHeavy02End = value;
                    break;
                case StateBool.lockTurn:
                    actorController.lockTurn = value;
                    break;
                case StateBool.rolling:
                    actorController.rolling = value;
                    break;
                case StateBool.backStep:
                    actorController.backStep = value;
                    break;
                case StateBool.inLeftLightAttack01:
                    playerAttacker.inLeftLightAttack01 = value;
                    break;
                case StateBool.inLeftLightAttack02:
                    playerAttacker.inLeftLightAttack02 = value;
                    break;
                // 添加其他的 bool 变量处理
            }
        }
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetBoolVariable(false);
        }

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