using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy.Services
{
    public class EnemyProjectilePoolManager
    {
        private readonly ObjectPool<EnemyProjectile> _objectPool;

        public EnemyProjectilePoolManager(EnemyProjectile projectilePrefab, int initialSize, int maxSize)
        {
            _objectPool = new ObjectPool<EnemyProjectile>(
                createFunc: () => Object.Instantiate(projectilePrefab),
                actionOnGet: obj => obj.gameObject.SetActive(true),
                actionOnRelease: obj => obj.gameObject.SetActive(false),
                actionOnDestroy: Object.Destroy,
                defaultCapacity: initialSize,
                maxSize: maxSize
            );
        }

        public EnemyProjectile GetFromPool(Vector3 position, Quaternion rotation)
        {
            var obj = _objectPool.Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        public void ReturnToPool(EnemyProjectile obj)
        {
            _objectPool.Release(obj);
        }
        
    }
}