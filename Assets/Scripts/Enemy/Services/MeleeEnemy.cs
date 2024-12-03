using System.Threading.Tasks;
using Enemy.Models;
using Player;
using Player.Controllers;
using Player.Views;
using UnityEngine;

namespace Enemy.Services
{
    public class MeleeEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;
        private bool _attacking = false;

        protected override void Start()
        {
            base.Start();
            _animationController.PlayAnimation("run");
        }

        public override async void Attack(PlayerView target)
        {
            if (_attacking) return;
            _attacking = true;
            // Debug.Log($"{gameObject.name} Damaged Player from distance {_meleeAttackerStats.DistanceToPlayer}");
            _animationController.PlayAnimation("attack");
            await Task.Delay((1000));
            _attacking = false;
            isAttacking = false;
            _animationController.PlayAnimation("run");
        }
    }
}