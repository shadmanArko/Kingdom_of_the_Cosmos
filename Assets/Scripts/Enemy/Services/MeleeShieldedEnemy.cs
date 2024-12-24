using System.Threading.Tasks;
using Enemy.Models;
using PlayerSystem;
using PlayerSystem.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy.Services
{
    public class MeleeShieldedEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;
        [SerializeField] private Slider _shieldSlider;
        private bool _attacking = false;
        public float maxShieldHealth;
        private float _shieldHealth;
    
        protected override void Start()
        {
            base.Start();
            _animationController.PlayAnimation("run");
        }

        public override void Initialize()
        {
            base.Initialize();
            _shieldHealth = maxShieldHealth;
            _shieldSlider.gameObject.SetActive(true);
            _shieldSlider.value = 1;
        }

        public override void TakeDamage(float amount)
        {
            if (_shieldHealth > 0)
            {
                _shieldHealth -= amount;
                _shieldSlider.value = 1 - (maxShieldHealth - _shieldHealth) / maxShieldHealth;
                Debug.Log($"Took shield Damage {amount}");
                if (_shieldHealth<=0)
                {
                    _shieldSlider.gameObject.SetActive(false);
                }
            }
            else
            {
                base.TakeDamage(amount);
            }
        
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
