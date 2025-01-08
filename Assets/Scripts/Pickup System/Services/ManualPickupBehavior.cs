namespace Pickup_System
{
    public class ManualPickupBehavior : IPickupBehavior
    {
        private readonly IPickupInputSystem inputSystem;

        public ManualPickupBehavior(IPickupInputSystem inputSystem)
        {
            this.inputSystem = inputSystem;
        }

        public bool CanPickup(IPickupCollector collector, IPickupable pickup)
        {
            return inputSystem.IsPickupButtonPressed;
        }
    }
}