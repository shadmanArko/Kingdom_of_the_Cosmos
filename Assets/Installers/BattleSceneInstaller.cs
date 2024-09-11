using GameData;
using Models;
using SaveAndLoad;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "BattleSceneInstaller", menuName = "Installers/BattleSceneInstaller")]
public class BattleSceneInstaller : ScriptableObjectInstaller<BattleSceneInstaller>
{
    public GameDataScriptable gameDataScriptable;
    
    public override void InstallBindings()
    {
        Container.Bind<GameDataScriptable>().FromInstance(gameDataScriptable).AsSingle();
        Container.Bind<GameDataLoader>().AsTransient().NonLazy();
    }
}