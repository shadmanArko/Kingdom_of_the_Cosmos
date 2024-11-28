using Player;
using UnityEditor.VersionControl;
using UnityEngine;
using Zenject;
using Task = System.Threading.Tasks.Task;

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

        public override async void Attack(PlayerController target)
        {
            EnemyProjectile projectile = enemyProjectilePoolManager.GetFromPool(transform.position, transform.rotation);
            _animationController.PlayAnimation("attack");
            projectile.SetStats(20, 20, target.transform, enemyProjectilePoolManager);
            await Task.Delay(500);
            _animationController.PlayAnimation("run");
            isAttacking = false;
            Debug.Log("Ranged attack occured");           
        }
    }
}