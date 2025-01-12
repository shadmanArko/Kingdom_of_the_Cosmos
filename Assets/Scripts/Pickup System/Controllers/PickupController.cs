using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Pickup_System.Manager;

namespace Pickup_System
{
    public class PickupController : ITickable, IPickupSystem
    {
        private readonly List<IPickupable> activePickups = new List<IPickupable>();
        private readonly IPickupCollector collector;
        private readonly IPickupDistanceCalculator distanceCalculator;
        private readonly IPickupPoolManager pickupPoolManager;
        
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
            foreach (var pickup in activePickups.ToList())
            {
                if (pickup.PickupBehavior is AutoPickupBehavior)
                {
    
                    // Calculate direction to player in 2D
                    Vector2 directionToPlayer = ((Vector2)collector.Position - (Vector2)pickup.Position).normalized;
                    float distance = Vector2.Distance(collector.Position, pickup.Position);
    
                    // Calculate pull strength with a higher base value for 2D physics
                    float pullStrength = Mathf.Lerp(collector.MagnetStrength, 0f, distance / collector.MagnetRadius);
    
                    // Get and configure Rigidbody2D
                    var rb = pickup.PickupView.gameObject.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        // Make sure the Rigidbody2D is properly configured
                        rb.bodyType = RigidbodyType2D.Dynamic;
                        //rb.gravityScale = 1f;
                        rb.constraints = RigidbodyConstraints2D.None;
        
                        // Apply force with ForceMode2D.Force for continuous effect
                        rb.AddForce(directionToPlayer * pullStrength, ForceMode2D.Force);
        
                        // Optional: Add slight upward force to prevent ground sticking
                        rb.AddForce(Vector2.up * (pullStrength * 0.1f), ForceMode2D.Force);
                    }
                    else
                    {
                        Debug.LogError("Rigidbody is null");
                    }

                }
                if (ShouldAttemptPickup(pickup))
                {
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