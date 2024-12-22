using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Enemy.Services
{
    public class EnemyProjectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private Vector2 _targetPosition;
        private EnemyProjectilePoolManager _enemyProjectilePoolManager;
        private bool _canMove = false;
        
        public void SetStats(float damage, float speed, Vector2 targetPosition, EnemyProjectilePoolManager enemyProjectilePoolManager)
        {
            _damage = damage;
            _speed = speed;
            _targetPosition = targetPosition;
            _enemyProjectilePoolManager = enemyProjectilePoolManager;
            _canMove = true;
        }
        private void Update()
        {
            // Example movement logic

            if (_canMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
                {
                    OnHit();
                }
            }
            
        }
        
        private void OnHit()
        {
            // Logic for impact
            Debug.Log($"Projectile hit with {_damage} damage!");
            _canMove = false;

            // Return to pool
            //FindObjectOfType<ProjectileSpawner>().ReturnProjectile(gameObject);
            
            _enemyProjectilePoolManager.ReturnToPool(this);
        }
    }
}