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
            IPickupBehavior behavior) : base(radius, transform, behavior)
        {
            ExpValue = expValue;
        }
        
        public class Factory : PlaceholderFactory<float, float, Vector3, Quaternion, IPickupBehavior, ExpCrystal>
        {
            private readonly IPickupPoolManager poolManager;
            private readonly DiContainer container;

            [Inject]
            public Factory(IPickupPoolManager poolManager, DiContainer container)
            {
                this.poolManager = poolManager;
                this.container = container;
            }

            public override ExpCrystal Create(float expValue, float radius, Vector3 position, Quaternion rotation, IPickupBehavior behavior)
            {
                // Get pooled view
                var view = poolManager.GetFromPool(position, rotation);
                
                // Create the model
                var crystal = new ExpCrystal(expValue, radius, view.transform, behavior);
            
                // Inject dependencies into the view
                container.Inject(view, new object[] { crystal });

                return crystal;
            }
        }
    }
    
    
}