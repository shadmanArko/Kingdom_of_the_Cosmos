namespace Pickup_System
{
    public interface IPickupSystem
    {
        void RegisterPickup(IPickupable pickup);
        void UnregisterPickup(IPickupable pickup);
    }
}