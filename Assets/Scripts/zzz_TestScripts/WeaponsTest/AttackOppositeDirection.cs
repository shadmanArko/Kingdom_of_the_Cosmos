using DBMS.RunningData;
using ObjectPoolScripts;
using Player;
using UnityEngine;

namespace zzz_TestScripts.WeaponsTest
{
    public class AttackOppositeDirection : AttackBase
    {
        
        private BulletPoolingManager _bulletPoolingManager;
        private RunningDataScriptable _runningDataScriptable;
        private PlayerController _playerController;
        
        public AttackOppositeDirection(BulletPoolingManager bulletPoolingManager, RunningDataScriptable runningDataScriptable, PlayerController playerController)
        {
            _bulletPoolingManager = bulletPoolingManager;
            _runningDataScriptable = runningDataScriptable;
            _playerController = playerController;
        }
        public override void Attack()
        {
            var bullet = _bulletPoolingManager.Pool.Get();
            
            bullet.transform.position = _playerController.transform.position;
            
            var direction = _runningDataScriptable.attackDirection;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            bullet.Initialize(_bulletPoolingManager.Pool, direction);
        }
    }
}
