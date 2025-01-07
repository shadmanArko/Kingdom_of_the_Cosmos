using UnityEngine;

namespace Pickup_System.Factory
{
    public interface IPickupFactory
    {
        PickupView CreatePickup(Vector3 position, Quaternion rotation);
    }
}