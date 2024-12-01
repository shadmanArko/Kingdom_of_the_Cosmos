using System.Collections.Generic;
using UnityEngine;

namespace ObjectPoolScripts
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int poolSize;

        private Queue<GameObject> _pool;
        private void Start()
        {
            _pool = new Queue<GameObject>();
            PopulatePool();
        }

        private void PopulatePool()
        {
            for (var i = 0; i < poolSize; i++)
            {
                var bullet = Instantiate(prefab, transform);
                bullet.SetActive(false);
                _pool.Enqueue(bullet);
            }
        }

        public GameObject GetObject()
        {
            Debug.Log($"bullet count: {_pool.Count}");
            if (_pool.Count > 0)
            {
                var bullet = _pool.Dequeue();
                bullet.SetActive(true);
                return bullet;
            }
            else
            {
                var bullet = Instantiate(prefab, transform);
                return bullet;
            }
        }

        public void ReturnObject(GameObject bullet)
        {
            _pool.Enqueue(bullet);
            bullet.SetActive(false);
            bullet.transform.position = Vector3.zero;
        }
    }
}
