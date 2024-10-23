using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CombatStanceState : State
    {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;
        public DeadState deadState;
        public float currentRight;
        public bool lefttoRightFlag;
        public bool TransitionCoroutineFlag=false;
        float waitTime=0;
        float t=0;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {  if (enemyAnimatorManager.animator.GetBool("isDead"))
            {
                return deadState;
            }
            
            
            enemyAnimatorManager.animator.SetBool("isIdle",false);
           
            float distanceFromTarget=Vector3.Distance(enemyManager.currentTarget.transform.position,enemyManager.transform.position);
            
            if (enemyManager.currentRecoveryTime <= 0 && enemyManager.distanceFromTarget <= enemyManager.maxAttackRange)
            {
                
             
                return attackState;
            }
            else if( distanceFromTarget>enemyManager.maxAttackRange)
            {
               
                return pursueTargetState;
            }
            else
            {   enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                pursueTargetState.HandleRotateTowardsTarget(enemyManager);
               
                if (distanceFromTarget <= 2f)
                { 
                    
                    enemyAnimatorManager.animator.SetFloat("forward",-1.5f,0.1f, Time.deltaTime);
                    enemyAnimatorManager.animator.SetFloat("right", 0,0.1f, Time.deltaTime);
                }
                else
                {
                   
                
                    if (TransitionCoroutineFlag == false)
                    {   waitTime = Random.Range(1, 4f);
                        float selectedNumber = (Random.value < 0.5f) ? -1.5f : 1.5f;
                    //    Debug.Log(selectedNumber);
                        StartCoroutine(TransitionToValue(selectedNumber));
                        t = 0;
                    }
                    t+=Time.deltaTime;
                    if (t > waitTime)
                    {
                        TransitionCoroutineFlag = false;
                    }
                   
                    enemyAnimatorManager.animator.SetFloat("right",currentRight,0.1f, Time.deltaTime);
                    
                }
                enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
                enemyManager.navMeshAgent.transform.localPosition=Vector3.zero;
                return this;
            }
          
        }
        
     

        IEnumerator TransitionToValue(float targetValue)
        {    TransitionCoroutineFlag=true;
            float elapsedTime = 0f;
            float initialValue = currentRight;
            
            while (elapsedTime < 1f)
            {
                // 根据过渡时间插值计算当前值
                currentRight = Mathf.Lerp(initialValue, targetValue, 0.1f);
                elapsedTime += Time.deltaTime;

                // 可以在这里对 currentValue 做其他操作，例如应用到对象的属性
            //    Debug.Log(targetValue);
                yield return null;
            }

            // 确保过渡结束时 currentValue 完全达到目标值
            currentRight = targetValue;
        }

   
 
    }
}