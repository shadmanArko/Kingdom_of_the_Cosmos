using UnityEngine;
using Zenject;

namespace Pickup_System
{
    public class PickupSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Core Systems
            Container.BindInterfacesTo<PickupController>().AsSingle();
            Container.Bind<IPickupDistanceCalculator>().To<PickupDistanceCalculator>().AsSingle();
        
            // Pickup Behaviors
            Container.Bind<AutoPickupBehavior>().AsSingle();
            Container.Bind<ManualPickupBehavior>().AsSingle();
        
            // Animation Handler
            Container.Bind<IPickupAnimationHandler>().To<DefaultPickupAnimationHandler>().AsSingle();
        
            // Factories
            //Container.BindFactory<float, float, RuleTile.TilingRuleOutput.Transform, IPickupBehavior, ExpCrystal, ExpCrystal.Factory>();
            //Container.BindFactory<string, float, Transform, IPickupBehavior, InventoryItem, InventoryItem.Factory>();
        }
    }
}