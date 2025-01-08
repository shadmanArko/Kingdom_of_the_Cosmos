using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pickup_System
{
    public interface IPickupAnimationHandler
    {
        UniTask PlayPickupAnimation(Transform pickupTransform);
    }
}