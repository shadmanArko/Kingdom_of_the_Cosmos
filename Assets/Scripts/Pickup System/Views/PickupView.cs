using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Pickup_System
{
    public class PickupView : MonoBehaviour
    {
        private IPickupable pickup;
        private IPickupAnimationHandler animationHandler;
        private IDisposable collectedSubscription;

        [Inject]
        public void Initialize(IPickupable pickup, IPickupAnimationHandler animationHandler)
        {
            this.pickup = pickup;
            this.animationHandler = animationHandler;
            SetupSubscriptions();
        }

        private void SetupSubscriptions()
        {
            var basePickup = pickup as BasePickupModel;
            if (basePickup != null)
            {
                collectedSubscription = basePickup.IsCollected
                    .Subscribe(collected =>
                    {
                        if (collected)
                        {
                            HandlePickupCollected().Forget();
                        }
                    });
            }
        }

        private async UniTaskVoid HandlePickupCollected()
        {
            await animationHandler.PlayPickupAnimation(transform);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            collectedSubscription?.Dispose();
        }
    }
}