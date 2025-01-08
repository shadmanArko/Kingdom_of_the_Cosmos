namespace Pickup_System
{
    public class AutoPickupBehavior : IPickupBehavior
    {
        public bool CanPickup(IPickupCollector collector, IPickupable pickup)
        {
            return true; // Auto pickup when in range
        }
    }
}