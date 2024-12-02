using System.Threading.Tasks;
using DBMS.RunningData;
using ObjectPoolScripts;
using Player;
using UnityEngine;
using Zenject;

namespace zzz_TestScripts.WeaponsTest
{
    public class ShootOnceInMouseDirection : AttackBase
    {
        [Inject] private BulletPoolingManager _bulletPoolingManager;
        [Inject] private RunningDataScriptable _runningDataScriptable;
        [Inject] private PlayerController _playerController;

        public ShootOnceInMouseDirection(BulletPoolingManager bulletPoolingManager, RunningDataScriptable runningDataScriptable, PlayerController playerController)
        {
            _bulletPoolingManager = bulletPoolingManager;
            _runningDataScriptable = runningDataScriptable;
            _playerController = playerController;
        }

        public override void Attack()
        {
            var bullet = _bulletPoolingManager.Pool.Get();
            var attackerPos = _playerController.gameObject.transform.position;
            bullet.transform.position = attackerPos;
            
            var direction = _runningDataScriptable.attackDirection;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            bullet.Initialize(_bulletPoolingManager.Pool, direction);

            // await ShootBullet(manager, attackerPos);

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
