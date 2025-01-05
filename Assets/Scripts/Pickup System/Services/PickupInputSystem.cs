using UniRx;
using UnityEngine;

namespace Pickup_System
{
    public class PickupInputSystem : IPickupInputSystem
    {
        private readonly ReactiveProperty<bool> isPickupButtonPressed = new ReactiveProperty<bool>(false);
        public bool IsPickupButtonPressed => isPickupButtonPressed.Value;

        public void Update()
        {
            isPickupButtonPressed.Value = Input.GetKey(KeyCode.E);
        }
    }
}