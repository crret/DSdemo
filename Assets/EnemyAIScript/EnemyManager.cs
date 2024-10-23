using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{


    public class EnemyManager :CharacterManager
    {   
        public bool isPerformingAction = false;
        public EnemyAnimatorManager enemyAnimatorManager;
        public State currentState;
        public EnemyStats enemyStats;
        public NavMeshAgent navMeshAgent; 
        public CharacterStats currentTarget;

        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAction;
        public bool isInteracting = false;
        
        [Header("AI settings")]
        public float detectionRadius=20f;
        public float minDetectionAngle=-50f;
        public float maxDetectionAngle=50f;
        public bool enableNavmesh = false;
        public float distanceFromTarget;
        public float currentRecoveryTime = 0;
        public float rotationSpeed=15f;
        public float maxAttackRange = 1.5f;
        public float searchRange = 10f;
        public float viewableAngle;
        private void Awake()
        {
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        }

        private void Start()
        {
         
        }

        private void Update()
        {
            navMeshAgent.enabled = enableNavmesh;
            isInteracting=enemyAnimatorManager.animator.GetBool("isInteracting");
            CheckIfInNavMesh();
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
            HandleRecoveryTimer();
        }

        public void HandleStateMachine()
        {
            if (currentState != null)
            {
                State nextState = currentState.Tick(this,enemyStats,enemyAnimatorManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
           
        }

        public void SwitchToNextState(State state)
        {
            currentState=state;
        }
        private void  CheckIfInNavMesh()
        {
            if (navMeshAgent.enabled == false)
            {
                return ;
            }
            NavMeshHit hit;
            bool isInNavMesh;
            float checkRadius = 0.2f;
            // 在目标当前位置进行 NavMesh.SamplePosition 检测
            if (currentTarget != null)
            {


                if (NavMesh.SamplePosition(currentTarget.transform.position, out hit, checkRadius, NavMesh.AllAreas))
                {
                    navMeshAgent.isStopped = false; // 目标在 NavMesh 上
                }
                else
                {
                    navMeshAgent.isStopped = true; // 目标不在 NavMesh 上// 通知敌人停止导航
                }
            }

        }

        

        private void HandleRecoveryTimer()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <=0)
                {
                    isPerformingAction = false;
                }
            }
        }
    }   
}