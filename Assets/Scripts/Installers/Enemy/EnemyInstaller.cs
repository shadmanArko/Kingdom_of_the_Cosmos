using Enemy.Manager;
using Enemy.Services;
using UnityEngine;
using Zenject;

namespace Installers.Enemy
{
    [CreateAssetMenu(fileName = "EnemyInstaller", menuName = "Installers/EnemyInstaller")]
    public class EnemyInstaller : ScriptableObjectInstaller<EnemyInstaller>
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private ComputeShader enemyComputeShader;
        [SerializeField] private MeleeEnemyPool meleeEnemyPool;
        [SerializeField] private MeleeShieldedEnemyPool meleeShieldedEnemyPool;
        [SerializeField] private RangedEnemyPool rangedEnemyPool;
        [SerializeField] private ShamanEnemyPool shamanEnemyPool;
        [SerializeField] private GameStatUI gameStatUI;
        [SerializeField] private EnemyProjectile enemyProjectilePrefab;
        [SerializeField] private int enemyProjectilePoolInitialSize;
        [SerializeField] private int enemyProjectilePoolMaxSize;

        public override void InstallBindings()
        {
            Container.BindInstance(enemyComputeShader);
            Container.BindInstance(enemyPrefab);
        
            Container.Bind<MeleeEnemyPool>().FromComponentInNewPrefab(meleeEnemyPool).AsSingle().NonLazy();
            Container.Bind<MeleeShieldedEnemyPool>().FromComponentInNewPrefab(meleeShieldedEnemyPool).AsSingle().NonLazy();
            Container.Bind<RangedEnemyPool>().FromComponentInNewPrefab(rangedEnemyPool).AsSingle().NonLazy();
            Container.Bind<ShamanEnemyPool>().FromComponentInNewPrefab(shamanEnemyPool).AsSingle().NonLazy();
            Container.Bind<GameStatUI>().FromComponentInNewPrefab(gameStatUI).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EnemyManager>().AsSingle();
            Container.Bind<EnemyProjectilePoolManager>().AsSingle().WithArguments(enemyProjectilePrefab, enemyProjectilePoolInitialSize, enemyProjectilePoolMaxSize);
        }
    }
}
