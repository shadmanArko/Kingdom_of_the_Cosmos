using Enemy.Manager;
using PlayerSystem.Views;
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
        private Transform _target;
        private EnemyProjectilePoolManager _enemyProjectilePoolManager;
        private bool _canMove = false;
        
        public void SetStats(float damage, float speed, Transform target, Vector2 targetPosition, EnemyProjectilePoolManager enemyProjectilePoolManager)
        {
            _damage = damage;
            _speed = speed;
            _target = target;
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
            float finalDistance = Vector3.Distance(transform.position, _target.transform.position);
            if (finalDistance < 0.1f && _target.GetComponent<PlayerView>())
            {
                EnemyManager.EnemyDamagedPlayer?.Invoke(_damage);
            }
            // Return to pool
            //FindObjectOfType<ProjectileSpawner>().ReturnProjectile(gameObject);
            
            _enemyProjectilePoolManager.ReturnToPool(this);
        }
    }
}