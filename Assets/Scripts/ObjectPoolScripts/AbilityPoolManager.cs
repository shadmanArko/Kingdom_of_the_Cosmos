using System.Collections.Generic;
using ObjectPool;
using PlayerScripts;
using TestScripts.WeaponsTest;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace ObjectPoolScripts
{
    public class AbilityPoolManager : MonoBehaviour
    {
        private BulletPoolingManager _bulletPoolingManager;
        private PlayerController _playerController;
        
        public ObjectPool<Ability> Pool { get; private set; }
        [SerializeField] private Ability abilityPrefab;

        [SerializeField] private int defaultPoolSize = 10;
        [SerializeField] private int maxPoolSize = 20;

        [SerializeField] private List<AttackBase> attackBases;

        [Inject]
        private void InitializeDiReference(BulletPoolingManager bulletPoolingManager, PlayerController playerController)
        {
            _bulletPoolingManager = bulletPoolingManager;
            _playerController = playerController;
        }

        private void Start()
        {
            Pool = new ObjectPool<Ability>(CreateAbility, OnGetAbilityFromPool, OnReleaseAbilityToPool,
                OnDestroyAbility, true, defaultPoolSize, maxPoolSize);
        }

        private Ability CreateAbility()
        {
            var ability = Instantiate(abilityPrefab, transform);
            ability.SetPool(Pool);
            return ability;
        }
        
        public void ActivateAbility(Vector3 position, Vector2 mousePos)
        {
            var ability = Pool.Get();
            ability.transform.position = position;
            ability.Activate(_bulletPoolingManager, _playerController, attackBases[Random.Range(0,attackBases.Count)], mousePos);
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
