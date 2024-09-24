using System.Collections.Generic;
using DBMS.RunningData;
using ObjectPool;
using PlayerScripts;
using Signals.BattleSceneSignals;
using TestScripts.WeaponsTest;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace ObjectPoolScripts
{
    public class AbilityPoolManager : MonoBehaviour
    {
        private SignalBus _signalBus;
        
        private BulletPoolingManager _bulletPoolingManager;
        private PlayerController _playerController;
        private RunningDataScriptable _runningDataScriptable;
        
        public ObjectPool<Ability> Pool { get; private set; }
        [SerializeField] private Ability abilityPrefab;

        [SerializeField] private int defaultPoolSize = 10;
        [SerializeField] private int maxPoolSize = 20;

        [SerializeField] private List<AttackBase> attackBases;

        [Inject]
        private void InitializeDiReference(BulletPoolingManager bulletPoolingManager, PlayerController playerController, SignalBus signalBus, RunningDataScriptable runningDataScriptable)
        {
            _bulletPoolingManager = bulletPoolingManager;
            _playerController = playerController;
            _runningDataScriptable = runningDataScriptable;
            _signalBus = signalBus;
        }

        private void Start()
        {
            Pool = new ObjectPool<Ability>(CreateAbility, OnGetAbilityFromPool, OnReleaseAbilityToPool,
                OnDestroyAbility, true, defaultPoolSize, maxPoolSize);
            
            _signalBus.Subscribe<MeleeAttackSignal>(ActivateAbility);
        }

        private Ability CreateAbility()
        {
            var ability = Instantiate(abilityPrefab, transform);
            ability.SetPool(Pool);
            return ability;
        }
        
        private void ActivateAbility()
        {
            var ability = Pool.Get();
            ability.transform.position = _playerController.gameObject.transform.position;
            // var attackAbility = new ShootOnceInMouseDirection(_bulletPoolingManager, _runningDataScriptable, _playerController);
            var attackAbility =
                new AttackOppositeDirection(_bulletPoolingManager, _runningDataScriptable, _playerController);
            ability.Activate(attackAbility);
        }

        private void OnGetAbilityFromPool(Ability ability)
        {
            ability.gameObject.SetActive(true);
        }

        private void OnReleaseAbilityToPool(Ability ability)
        {
            ability.gameObject.SetActive(false);
        }

        private static void OnDestroyAbility(Ability ability)
        {
            Destroy(ability.gameObject);
        }
    }
}
