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
    [SerializeField] private EnemySpawnSettings spawnSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(enemyComputeShader);
        Container.BindInstance(enemyPrefab);
        Container.BindInstance(spawnSettings);
        
        

        // Then bind MeleeEnemyPool
        Container.Bind<MeleeEnemyPool>().FromComponentInNewPrefab(meleeEnemyPool).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<EnemyManager>().AsSingle();
    }
}
[System.Serializable]
public class EnemySpawnSettings
{
    public int enemiesPerWave = 10;
    public int increaseEnemiesPerWave = 3;
    public float timeBetweenWaves = 5f;
    public float spawnRadius = 10f;
    public float forwardSpawnBias = 0.7f;
}