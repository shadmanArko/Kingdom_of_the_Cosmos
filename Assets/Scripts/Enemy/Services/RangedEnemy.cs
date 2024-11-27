using Player;
using UnityEngine;
using Zenject;

namespace Enemy.Services
{
    public class RangedEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;
        public EnemyProjectilePoolManager enemyProjectilePoolManager;
        
        // [Inject]
        // public void Construct(EnemyProjectilePoolManager enemyProjectilePoolManager)
        // {
        //     _enemyProjectilePoolManager = enemyProjectilePoolManager;
        //     Debug.LogError(_enemyProjectilePoolManager == null
        //         ? "enemyProjectilePoolManager in Null"
        //         : "enemyProjectilePoolManager is not null");
        // }

        protected override void Start()
        {
            base.Start();
            _animationController.PlayAnimation("run");
        }

        public override void Attack(PlayerController target)
        {
            EnemyProjectile projectile = enemyProjectilePoolManager.GetFromPool(transform.position, transform.rotation);
            projectile.SetStats(20, 20, target.transform, enemyProjectilePoolManager);

            Debug.Log("Ranged attack occured");           
        }
    }
}