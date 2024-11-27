using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Enemy.Services
{
    public class EnemyProjectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private Transform _target;
        private EnemyProjectilePoolManager _enemyProjectilePoolManager;
        
        public void SetStats(float damage, float speed, Transform target, EnemyProjectilePoolManager enemyProjectilePoolManager)
        {
            _damage = damage;
            _speed = speed;
            _target = target;
            _enemyProjectilePoolManager = enemyProjectilePoolManager;
        }
        private void Update()
        {
            // Example movement logic
            if (_target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, _target.position) < 0.1f)
                {
                    OnHit();
                }
            }
        }
        
        private void OnHit()
        {
            // Logic for impact
            Debug.Log($"Projectile hit with {_damage} damage!");
            _target = null;

            // Return to pool
            //FindObjectOfType<ProjectileSpawner>().ReturnProjectile(gameObject);
            
            _enemyProjectilePoolManager.ReturnToPool(this);
        }
    }
}