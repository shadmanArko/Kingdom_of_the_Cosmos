using System;
using Player;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "EnemyInstaller", menuName = "Installers/EnemyInstaller")]
public class EnemyInstaller : ScriptableObjectInstaller<EnemyInstaller>
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private ComputeShader enemyComputeShader;
    [SerializeField] private MeleeEnemyPool meleeEnemyPool;
    [SerializeField] private GameStatUI gameStatUI;

    public override void InstallBindings()
    {
        Container.BindInstance(enemyComputeShader);
        Container.BindInstance(enemyPrefab);
        
        Container.Bind<MeleeEnemyPool>().FromComponentInNewPrefab(meleeEnemyPool).AsSingle().NonLazy();
        Container.Bind<GameStatUI>().FromComponentInNewPrefab(gameStatUI).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<EnemyManager>().AsSingle();
    }
}
