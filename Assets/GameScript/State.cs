using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;

namespace SG
{
  
  public abstract class State : MonoBehaviour
  {
    public abstract State Tick(EnemyManager enemyManager, EnemyStats enemyStats,
      EnemyAnimatorManager enemyAnimatorManager);
  }
}