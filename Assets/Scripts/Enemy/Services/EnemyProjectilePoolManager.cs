using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy.Services
{
    public class EnemyProjectilePoolManager
    {
        private readonly ObjectPool<GameObject> _objectPool;

        public EnemyProjectilePoolManager(GameObject projectilePrefab, int initialSize, int maxSize)
        {
            _objectPool = new ObjectPool<GameObject>(
                createFunc: () => Object.Instantiate(projectilePrefab),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: Object.Destroy,
                defaultCapacity: initialSize,
                maxSize: maxSize
            );
        }

        public GameObject GetFromPool(Vector3 position, Quaternion rotation)
        {
            var obj = _objectPool.Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        public void ReturnToPool(GameObject obj)
        {
            _objectPool.Release(obj);
        }
        
    }
}