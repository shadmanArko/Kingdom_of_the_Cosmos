using UnityEngine;
using UnityEngine.Pool;

namespace Pickup_System.Manager
{
    public class PickupPoolManager : IPickupPoolManager
    {
        private readonly ObjectPool<PickupView> _objectPool;
        private readonly PickupView _pickupPrefab;

        public PickupPoolManager(PickupView pickupPrefab)
        {
            _pickupPrefab = pickupPrefab;
            _objectPool = new ObjectPool<PickupView>(
                createFunc: ()=> Object.Instantiate(_pickupPrefab),
                actionOnGet: obj => obj.gameObject.SetActive(true),
                actionOnRelease: obj => obj.gameObject.SetActive(false),
                actionOnDestroy: Object.Destroy,
                defaultCapacity: 10,
                maxSize: 1000
            );
        }

        public PickupView GetFromPool(Transform transform)
        {
            var obj = _objectPool.Get();
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            return obj;
        }

        public void ReturnToPool(PickupView pickupView)
        {
            _objectPool.Release(pickupView);
        }
    }
}