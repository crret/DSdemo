using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class IdleState : State
    {   public PursueTargetState pursueTargetState;
        public Collider selfCollider;
        public LayerMask detectionLayer;
        public DeadState deadState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats,
            EnemyAnimatorManager enemyAnimatorManager)
        {   if (enemyAnimatorManager.animator.GetBool("isDead"))
            {
                return deadState;
            }
            enemyAnimatorManager.animator.SetBool("isIdle",true);
            #region Handle Enemy Target Detection
            Collider[]  colliders = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.detectionRadius,detectionLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != selfCollider)
                {
                    CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

                    if (characterStats != null)
                    {
                        Vector3 targetDirection = characterStats.transform.position - enemyManager.transform.position;
                        float viewableAngle = Vector3.Angle(enemyManager.transform.forward, targetDirection);
                        if (viewableAngle > enemyManager.minDetectionAngle &&
                            viewableAngle < enemyManager.maxDetectionAngle)
                        {
                            enemyManager.currentTarget = characterStats;
                            Debug.Log(characterStats);
                        }
                    }
                }
            }
            #endregion
            
            #region Handle Switching to next state
            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }
            #endregion
        }
    }
}