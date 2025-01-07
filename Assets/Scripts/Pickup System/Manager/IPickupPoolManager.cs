using UnityEngine;

namespace Pickup_System.Manager
{
    public interface IPickupPoolManager
    {
        PickupView GetFromPool(Vector3 position, Quaternion rotation);
        void ReturnToPool(PickupView pickupView);
    }
}