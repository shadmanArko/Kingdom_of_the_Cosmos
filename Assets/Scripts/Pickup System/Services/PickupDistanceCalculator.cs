using UnityEngine;

namespace Pickup_System
{
    public class PickupDistanceCalculator : IPickupDistanceCalculator
    {
        public bool IsInRange(IPickupCollector collector, IPickupable pickup)
        {
            return Vector3.Distance(collector.Position, pickup.Position) <= pickup.PickupRadius;
        }
    }
}