using UnityEngine;

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
    }
}