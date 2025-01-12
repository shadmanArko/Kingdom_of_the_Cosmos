using System.Collections.Generic;
using System.Linq;
using Pickup_System.Manager;
using UnityEngine;
using Zenject;

namespace Pickup_System
{
    public class PickupController : ITickable, IPickupSystem
    {
        private readonly HashSet<IPickupable> activePickups = new HashSet<IPickupable>();
        private readonly IPickupCollector collector;
        private readonly IPickupDistanceCalculator distanceCalculator;
        private readonly IPickupPoolManager pickupPoolManager;

        [Inject]
        public PickupController(
            IPickupCollector collector,
            IPickupDistanceCalculator distanceCalculator, IPickupPoolManager pickupPoolManager)
        {
            this.collector = collector;
            this.distanceCalculator = distanceCalculator;
            this.pickupPoolManager = pickupPoolManager;
        }

        public void RegisterPickup(IPickupable pickup)
        {
            activePickups.Add(pickup);
        }

        public void UnregisterPickup(IPickupable pickup)
        {
            activePickups.Remove(pickup);
        }

        public void Tick()
        {
            foreach (var pickup in activePickups.ToArray())
            {
                if (ShouldAttemptPickup(pickup))
                {
                    Debug.Log("Pick Up In process.....");
                    ProcessPickup(pickup);
                }
            }
        }

        private bool ShouldAttemptPickup(IPickupable pickup)
        {
            return distanceCalculator.IsInRange(collector, pickup);
        }

        private void ProcessPickup(IPickupable pickup)
        {
            if (pickup.CanBePickedUp(collector) && 
                collector.CanCollectPickup(pickup))
            {
                pickup.OnPickup(collector);
                collector.CollectPickup(pickup);
                pickupPoolManager.ReturnToPool(pickup.PickupView);
                UnregisterPickup(pickup);
            }
        }
    }
}