

using UnityEngine;

namespace Pickup_System
{
    public interface IPickupCollector
    {
        Vector3 Position { get; }
        bool CanCollectPickup(IPickupable pickup);
        void CollectPickup(IPickupable pickup);
    }
}