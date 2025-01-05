namespace Pickup_System
{
    public interface IPickupBehavior
    {
        bool CanPickup(IPickupCollector collector, IPickupable pickup);
    }
}