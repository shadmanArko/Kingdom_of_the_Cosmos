using Pickup_System.Manager;
using UnityEngine;
using Zenject;

namespace Pickup_System
{
    public class ExpCrystal : BasePickupModel
    {
        public float ExpValue { get; }

        public ExpCrystal(
            float expValue, 
            float radius, 
            Transform transform,
            PickupView pickupView,
            IPickupBehavior behavior) : base(radius, transform, behavior, pickupView)
        {
            ExpValue = expValue;
        }
        
        public class Factory : PlaceholderFactory<float, float, Transform, IPickupBehavior, ExpCrystal>
        {
            private readonly IPickupPoolManager poolManager;
            private readonly DiContainer container;
            private readonly PickupView pickupViewPrefab;

            [Inject]
            public Factory(
                IPickupPoolManager poolManager, 
                DiContainer container,
                PickupView pickupViewPrefab)  // Add prefab injection
            {
                this.poolManager = poolManager;
                this.container = container;
                this.pickupViewPrefab = pickupViewPrefab;
            }

            public override ExpCrystal Create(float expValue, float radius, Transform transform, IPickupBehavior behavior)
            {
                // Get pooled view
                var view = poolManager.GetFromPool(transform);
                
                // Create the model
                var crystal = new ExpCrystal(expValue, radius, view.transform, view, behavior);
            
                // Inject dependencies into the view
                //container.Inject(view, new object[] { crystal });

                return crystal;
            }
        }
    }
    
    
}