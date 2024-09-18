using System.Collections;
using System.Threading.Tasks;
using ObjectPool;
using UnityEngine;

namespace TestScripts.WeaponsTest
{
    [CreateAssetMenu(fileName = "AttackOneDirection", menuName = "Testing/OneDirection")]
    public class AttackOneDirection : AttackBase
    {
        public override async void Attack(BulletPoolingManager manager, Vector2 attackerPos, Vector2 mousePos)
        {
            // var bullet = manager.Pool.Get();
            //
            // bullet.transform.position = attackerPos;
            //
            // var direction = mousePos - attackerPos;
            // var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            // bullet.Initialize(manager.Pool, direction);

            await ShootBullet(manager, attackerPos);

        }

        private async Task ShootBullet(BulletPoolingManager manager, Vector2 attackerPos)
        {
            var spreadAngle = Random.Range(90, 180);
            var bulletCount = 10;

            for (int i = 0; i < bulletCount; i++)
            {
                var angleStep = spreadAngle / (bulletCount - 1f);
                var startAngle = -spreadAngle / 2f;

                var angle = startAngle + angleStep * i;
                var direction = Quaternion.Euler(0, 0, angle) * Vector3.up;

                var bullet = manager.Pool.Get();
                bullet.transform.position = attackerPos;
                bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
                bullet.Initialize(manager.Pool, direction);
                await Task.Delay(1000);
            }
        }
        
    }
}
