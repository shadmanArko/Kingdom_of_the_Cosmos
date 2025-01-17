﻿using Enemy.Services;
using PlayerSystem.Views;
using UnityEngine;

namespace Enemy.Models
{
     public interface IEnemy
     {
          void Move(Vector2 targetPosition);
          void MoveTowardsTarget(Transform targetTransform);
          void Attack(PlayerView target);
          void TakeDamage(float amount);
          void TakeKnockBack(Transform fromTransform, float strength, float duration);
          void GetBuff(EnemyBuffTypes buffType, float amount, float duration);
     }
}
