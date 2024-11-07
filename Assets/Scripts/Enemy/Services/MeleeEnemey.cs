using System;
using Player;
using UnityEngine;
using UnityEngine.Pool;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;
    private MeleeAttacker _meleeAttackerStats;
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
    public void HandleAttack(PlayerController playerController)
    {
        Debug.Log($"{gameObject.name} Damaged Player from distance {_meleeAttackerStats.DistanceToPlayer}");
    }
}