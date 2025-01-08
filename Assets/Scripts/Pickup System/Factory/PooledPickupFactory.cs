using Pickup_System.Manager;
using UnityEngine;
using Zenject;

namespace Pickup_System.Factory
{
    public class PooledPickupFactory : IPickupFactory
    {
        private readonly IPickupPoolManager _poolManager;
        private readonly DiContainer _container;

        public PooledPickupFactory(IPickupPoolManager poolManager, DiContainer container)
        {
            this._poolManager = poolManager;
            this._container = container;
        }

        public PickupView CreatePickup(Vector3 position, Quaternion rotation)
        {
            var pickup = _poolManager.GetFromPool(position, rotation);
            // Re-inject dependencies
            _container.Inject(pickup);
            
            return pickup;
        }
    }
}