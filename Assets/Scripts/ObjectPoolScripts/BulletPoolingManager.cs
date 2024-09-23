using Projectiles;
using UnityEngine;
using UnityEngine.Pool;

namespace ObjectPoolScripts
{
    public class BulletPoolingManager : MonoBehaviour
    {
        public ObjectPool<Bullet> Pool;
        [SerializeField] private Bullet bulletPrefab;

        private void Start()
        {
            Pool = new ObjectPool<Bullet>(CreateBullet, OnGetBulletFromPool, OnReleaseBulletToPool, OnDestroyBullet, true, 50, 100);
        }
        
        private Bullet CreateBullet()
        {
            var bullet = Instantiate(bulletPrefab, transform);
            bullet.SetPool(Pool);
            return bullet;
        }

        private void OnGetBulletFromPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        private void OnReleaseBulletToPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        private static void OnDestroyBullet(Bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

    }
}