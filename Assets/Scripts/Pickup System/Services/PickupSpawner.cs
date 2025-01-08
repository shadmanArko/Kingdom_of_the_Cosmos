using UnityEngine;

namespace Pickup_System
{
    public class PickupSpawner
    {
        private readonly ExpCrystal.Factory _expCrystalFactory;
        private readonly InventoryItem.Factory _inventoryItemFactory;
        private readonly IPickupBehavior _autoPickupBehavior;
        private readonly IPickupBehavior _manualPickupBehavior;
        private readonly IPickupSystem _pickupSystem;

        
        public PickupSpawner(
            ExpCrystal.Factory expCrystalFactory,
            InventoryItem.Factory inventoryItemFactory,
            AutoPickupBehavior autoPickupBehavior,
            ManualPickupBehavior manualPickupBehavior,
            IPickupSystem pickupSystem)
        {
            _expCrystalFactory = expCrystalFactory;
            _inventoryItemFactory = inventoryItemFactory;
            _autoPickupBehavior = autoPickupBehavior;
            _manualPickupBehavior = manualPickupBehavior;
            _pickupSystem = pickupSystem;
        }

        public ExpCrystal SpawnExpCrystal(Transform transform, float expValue)
        {
            // var tempTransform = new GameObject("TempTransform").transform;
            // tempTransform.position = position;

            var crystal = _expCrystalFactory.Create(
                expValue,
                2f,
                transform,
                _autoPickupBehavior
            );
            
            _pickupSystem.RegisterPickup(crystal);
            return crystal;
        }

        public InventoryItem SpawnInventoryItem(Transform transform, string itemId)
        {
            // var tempTransform = new GameObject("TempTransform").transform;
            // tempTransform.position = position;

            var item = _inventoryItemFactory.Create(
                itemId,
                1.5f,
               transform,
                _manualPickupBehavior
            );
            
            _pickupSystem.RegisterPickup(item);
            return item;
        }
    }
}