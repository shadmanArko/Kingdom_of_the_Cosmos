using UnityEngine;

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
    }
}