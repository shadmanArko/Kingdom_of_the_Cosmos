using Enemy.Services;
using Player;
using Player.Controllers;
using Player.Views;
using UnityEngine;

namespace Enemy.Models
{
     public interface IEnemy
     {
          void Move(Vector2 targetPosition);
          void MoveTowardsTarget(Transform targetTransform);
          void Attack(PlayerView target);
          void TakeDamage(float amount);
          void GetBuff(EnemyBuffTypes buffType, float amount, float duration);
     }
}
