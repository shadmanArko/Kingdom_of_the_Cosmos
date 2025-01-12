

using UnityEngine;

namespace Pickup_System
{
    public interface IPickupable
    {
        float PickupRadius { get; }
        Vector3 Position { get; }
        void OnPickup(IPickupCollector collector);
        bool CanBePickedUp(IPickupCollector collector);
        IPickupBehavior PickupBehavior { get; }
        PickupView PickupView { get; }
    }
}