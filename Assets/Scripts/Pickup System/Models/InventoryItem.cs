using Pickup_System.Manager;
using UnityEngine;
using Zenject;

namespace Pickup_System
{
    public class InventoryItem : BasePickupModel
    {
        public string ItemId { get; }

        public InventoryItem(
            string itemId, 
            float radius, 
            Transform transform,
            IPickupBehavior behavior) : base(radius, transform, behavior)
        {
            ItemId = itemId;
        }
        
        public class Factory : PlaceholderFactory<string, float, Transform, IPickupBehavior, InventoryItem>
        {
            private readonly IPickupPoolManager poolManager;
            private readonly DiContainer container;

            [Inject]
            public Factory(IPickupPoolManager poolManager, DiContainer container)
            {
                this.poolManager = poolManager;
                this.container = container;
            }

            public override InventoryItem Create(string itemId, float radius, Transform transform, IPickupBehavior behavior)
            {
                // Get pooled view
                var view = poolManager.GetFromPool(transform);

                // Create the model
                var item = new InventoryItem(itemId, radius, view.transform, behavior);
            
                // Inject dependencies into the view
                container.Inject(view, new object[] { item });

                return item;
            }
        }
    }
}