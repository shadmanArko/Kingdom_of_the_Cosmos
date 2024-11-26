using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Enemy.Services
{
    public class EnemyProjectilePoolManager
    {
        public class ProjectilePool
        {
            public string Type { get; }
            public GameObject Prefab { get; }
            public int InitialSize { get; }
            public int MaxSize { get; }

            public ProjectilePool(string type, GameObject prefab, int initialSize, int maxSize)
            {
                Type = type;
                Prefab = prefab;
                InitialSize = initialSize;
                MaxSize = maxSize;
            }
        }

        private readonly Dictionary<string, ObjectPool<GameObject>> _poolDictionary;

        public EnemyProjectilePoolManager(IEnumerable<ProjectilePool> projectilePools)
        {
            _poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

            foreach (var pool in projectilePools)
            {
                _poolDictionary[pool.Type] = new ObjectPool<GameObject>(
                    createFunc: () => Object.Instantiate(pool.Prefab),
                    actionOnGet: obj => obj.SetActive(true),
                    actionOnRelease: obj => obj.SetActive(false),
                    actionOnDestroy: Object.Destroy,
                    defaultCapacity: pool.InitialSize,
                    maxSize: pool.MaxSize
                );
            }
        }
        
        public GameObject GetFromPool(string type, Vector3 position, Quaternion rotation)
        {
            if (_poolDictionary.TryGetValue(type, out var pool))
            {
                var obj = pool.Get();
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                return obj;
            }
            Debug.LogError($"Pool of type {type} does not exist.");
            return null;
        }
        
        public void ReturnToPool(string type, GameObject obj)
        {
            if (_poolDictionary.TryGetValue(type, out var pool))
            {
                pool.Release(obj);
            }
            else
            {
                Debug.LogError($"Pool of type {type} does not exist.");
                Object.Destroy(obj); // Destroy the object if no pool exists to handle it.
            }
        }
        
    }
}