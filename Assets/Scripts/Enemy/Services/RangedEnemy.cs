using Player;
using UnityEngine;

namespace Enemy.Services
{
    public class RangedEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;

        protected override void Start()
        {
            base.Start();
            _animationController.PlayAnimation("run");
        }

        public override void Attack(PlayerController target)
        {
            Debug.Log("Ranged attack occured");           
        }
    }
}