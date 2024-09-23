using Signals.BattleSceneSignals;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "BattleSceneSignalInstaller", menuName = "Installers/BattleSceneSignalInstaller")]
    public class BattleSceneSignalInstaller : ScriptableObjectInstaller<BattleSceneSignalInstaller>
    {
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<MeleeAttackSignal>();
            Container.DeclareSignal<MouseMovementSignal>();
        }
    }
}