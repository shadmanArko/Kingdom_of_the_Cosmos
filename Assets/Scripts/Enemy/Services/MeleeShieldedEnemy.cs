using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Player;
using UnityEngine;

public class MeleeShieldedEnemy : BaseEnemy
{
    [SerializeField] private PlayerAnimationController _animationController;
    private bool _attacking = false;
    private float _shieldHealth = 20;

    protected override void Start()
    {
        base.Start();
        _animationController.PlayAnimation("run");
    }

    public override void TakeDamage(float amount)
    {
        if (_shieldHealth > 0)
        {
            _shieldHealth -= amount;
            Debug.Log($"Took shield Damage {amount}");

        }
        else
        {
            base.TakeDamage(amount);
        }
        
    }

    public override async void Attack(PlayerController target)
    {
        if (_attacking) return;
        _attacking = true;
        // Debug.Log($"{gameObject.name} Damaged Player from distance {_meleeAttackerStats.DistanceToPlayer}");
        _animationController.PlayAnimation("attack");
        await Task.Delay((1000));
        _attacking = false;
        _animationController.PlayAnimation("run");
    }
}
