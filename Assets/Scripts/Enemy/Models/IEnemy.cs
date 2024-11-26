using Player;
using UnityEngine;

public interface IEnemy
{
     void Move(Vector2 targetPosition);
     void MoveTowardsTarget(Transform targetTransform);
     void Attack(PlayerController target);
     void TakeDamage(float amount);
}
