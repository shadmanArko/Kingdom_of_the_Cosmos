using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pickup_System
{
    public class DefaultPickupAnimationHandler : IPickupAnimationHandler
    {
        public UniTask PlayPickupAnimation(Transform pickupTransform)
        {
            return new UniTask();
        }
    }
}