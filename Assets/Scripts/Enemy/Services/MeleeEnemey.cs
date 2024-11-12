using System;
using System.Threading.Tasks;
using Player;
using UnityEngine;
using UnityEngine.Pool;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;
    private MeleeAttacker _meleeAttackerStats;
    private bool _attacking = false;
    private void Start()
    {
        _animationController.PlayAnimation("run");
    }

    public void SetMeleeAttackerStat(MeleeAttacker meleeAttacker)
    {
        _meleeAttackerStats = meleeAttacker;
        transform.position = new Vector3(_meleeAttackerStats.Position.x, _meleeAttackerStats.Position.y, 0);
    }
    public MeleeAttacker GetMeleeAttackerStat()
    {
        return _meleeAttackerStats;
    }
    public async void HandleAttack(PlayerController playerController)
    {
        if (_attacking) return;
        _attacking = true;
        Debug.Log($"{gameObject.name} Damaged Player from distance {_meleeAttackerStats.DistanceToPlayer}");
        _animationController.PlayAnimation("attack");
        await Task.Delay((1000));
        _attacking = false;
        _animationController.PlayAnimation("run");
    }
}