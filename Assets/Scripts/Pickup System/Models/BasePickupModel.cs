using UniRx;
using UnityEngine;


namespace Pickup_System
{
    public abstract class BasePickupModel : IPickupable
    {
        public float PickupRadius { get; }
        public Vector3 Position => Transform.position;
        public IPickupBehavior PickupBehavior { get; }
        public PickupView PickupView { get; }
    
        protected Transform Transform { get; }
        private readonly ReactiveProperty<bool> isCollected = new ReactiveProperty<bool>(false);
        public IReadOnlyReactiveProperty<bool> IsCollected => isCollected;

        protected BasePickupModel(float radius, Transform transform, IPickupBehavior behavior, PickupView pickupView)
        {
            PickupRadius = radius;
            Transform = transform;
            PickupBehavior = behavior;
            PickupView = pickupView;
        }

        public virtual bool CanBePickedUp(IPickupCollector collector)
        {
            return !isCollected.Value && PickupBehavior.CanPickup(collector, this);
        }

        public virtual void OnPickup(IPickupCollector collector)
        {
            isCollected.Value = true;
        }
    }
}