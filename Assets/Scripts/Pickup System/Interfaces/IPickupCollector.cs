

using UnityEngine;

namespace Pickup_System
{
    public interface IPickupCollector
    {
        Vector3 Position { get; }
        float MagnetRadius { get; }
        float MagnetStrength { get; }
        bool CanCollectPickup(IPickupable pickup);
        void CollectPickup(IPickupable pickup);
    }
}