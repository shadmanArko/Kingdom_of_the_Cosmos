using UnityEngine;
using UnityEngine.Pool;
using ObjectPool;
using PlayerScripts;
using TestScripts.WeaponsTest;

namespace ObjectPoolScripts
{
    public class Ability : MonoBehaviour
    {
        private ObjectPool<Ability> _abilityPool;

        [SerializeField] private AttackBase attackBase;
        
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float duration = 2f;
        
        public void SetPool(ObjectPool<Ability> pool)
        {
            _abilityPool = pool;
        }

        public void Activate(BulletPoolingManager bulletPoolingManager, PlayerController playerController, AttackBase attackBaseType, Vector2 mousePos)
        {
            // StartCoroutine(ShootBullets(bulletCount, spreadAngle));
            attackBase = attackBaseType;
            if(attackBaseType == null) Debug.Log("attack base is null");
            attackBase.Attack(bulletPoolingManager, playerController.gameObject.transform.position, mousePos);
        }

        // private IEnumerator ShootBullets(int bulletCount, float spreadAngle)
        // {
        //     float angleStep = spreadAngle / (bulletCount - 1);
        //     float startAngle = -spreadAngle / 2;
        //
        //     for (int i = 0; i < bulletCount; i++)
        //     {
        //         float angle = startAngle + (angleStep * i);
        //         Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
        //
        //         var bullet = _bulletPoolingManager.Pool.Get();
        //         bullet.transform.position = _playerController.gameObject.transform.position;
        //         bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        //         bullet.Initialize(_bulletPoolingManager.Pool, direction);
        //     }
        //
        //     yield return new WaitForSeconds(duration);
        //     _abilityPool.Release(this);
        // }
    }

}
