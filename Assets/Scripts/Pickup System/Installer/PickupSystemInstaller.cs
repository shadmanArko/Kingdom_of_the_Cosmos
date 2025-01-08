using Pickup_System.Manager;
using PlayerSystem.Controllers;
using UnityEngine;
using Zenject;

namespace Pickup_System
{
    [CreateAssetMenu(fileName = "PickupSystemInstaller", menuName = "Installers/PickupSystemInstaller")]
    public class PickupSystemInstaller : ScriptableObjectInstaller<PickupSystemInstaller>
    {
        [SerializeField] private PickupView pickupView;
        public override void InstallBindings()
        {
            // Core Systems
            Container.BindInterfacesTo<PickupController>().AsSingle();
            Container.Bind<IPickupDistanceCalculator>().To<PickupDistanceCalculator>().AsSingle();
            Container.Bind<PickupSpawner>().AsSingle();
            Container.Bind<IPickupInputSystem>().To<PickupInputSystem>().AsSingle();
            
            // Pickup Behaviors
            Container.Bind<AutoPickupBehavior>().AsSingle();
            Container.Bind<ManualPickupBehavior>().AsSingle();
        
            // Animation Handler
            Container.Bind<IPickupAnimationHandler>().To<DefaultPickupAnimationHandler>().AsSingle();
        
            // Factories
            Container.BindFactory<float, float, Vector3, Quaternion, IPickupBehavior, ExpCrystal, ExpCrystal.Factory>();
            Container.BindFactory<string, float, Vector3, Quaternion, IPickupBehavior, InventoryItem, InventoryItem.Factory>();
            
            
            // Pool System
            Container.Bind<IPickupPoolManager>()
                .To<PickupPoolManager>()
                .AsSingle()
                .WithArguments(pickupView);

            // // Initialize pools
            // var poolManager = Container.Resolve<IPickupPoolManager>();
            // poolManager.RegisterPool("ExpCrystal", expCrystalPrefab, 20);
            // poolManager.RegisterPool("InventoryItem", inventoryItemPrefab, 10);

        }
    }
}