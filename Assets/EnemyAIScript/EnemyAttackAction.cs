using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/Enemy Actions/Attack Action")]
    public class EnemyAttackAction : EnemyAction
    {
        public int attackScore = 3;
        public float recoveryTime = 2f;

        public float maxAttackAngle = 35f;
        public float minAttackAngle = -35f;

        public float maxDistanceNeededToAttack = 0f;
        public float minDistanceNeededToAttack = 3f;
    }
}