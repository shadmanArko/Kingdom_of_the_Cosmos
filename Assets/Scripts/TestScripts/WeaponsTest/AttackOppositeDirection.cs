using ObjectPool;
using ObjectPoolScripts;
using UnityEngine;

namespace TestScripts.WeaponsTest
{
    public class AttackOppositeDirection : AttackBase
    {
        public override void Attack(BulletPoolingManager manager, Vector2 attackerPos, Vector2 mousePos)
        {
            var bullet = manager.Pool.Get();
            
            bullet.transform.position = attackerPos;
            
            var direction = (mousePos - attackerPos) * new Vector2(-1,-1);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            bullet.Initialize(manager.Pool, direction);
        }
    }
}
