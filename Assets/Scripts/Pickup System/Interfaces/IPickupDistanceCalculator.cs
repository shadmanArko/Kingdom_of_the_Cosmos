namespace Pickup_System
{
    public interface IPickupDistanceCalculator
    {
        bool IsInRange(IPickupCollector collector, IPickupable pickup);
    }
}