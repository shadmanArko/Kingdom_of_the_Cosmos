using UnityEngine;

namespace Pickup_System.Factory
{
    public interface IPickupFactory
    {
        PickupView CreatePickup(Transform transform);
    }
}