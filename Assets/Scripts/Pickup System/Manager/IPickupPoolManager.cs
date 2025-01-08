using UnityEngine;

namespace Pickup_System.Manager
{
    public interface IPickupPoolManager
    {
        PickupView GetFromPool(Transform transform);
        void ReturnToPool(PickupView pickupView);
    }
}