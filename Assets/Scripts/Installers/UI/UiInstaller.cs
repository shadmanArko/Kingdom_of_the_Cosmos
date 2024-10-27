using UnityEngine;
using Zenject;
using zzz_TestScripts.Signals.UiSignals;

namespace Installers.UI
{
    [CreateAssetMenu(fileName = "UiInstaller", menuName = "Installers/UiInstaller")]
    public class UiInstaller : ScriptableObjectInstaller<UiInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<MainMenuStartGameButtonSignal>();
            Container.DeclareSignal<SceneChangeSignal>();
        }
    }
}