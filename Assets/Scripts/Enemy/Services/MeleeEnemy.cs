﻿using System;
using System.Threading.Tasks;
using Player;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class MeleeEnemy : BaseEnemy
{
    [SerializeField] private PlayerAnimationController _animationController;
    private bool _attacking = false;

    protected override void Start()
    {
        base.Start();
        _animationController.PlayAnimation("run");
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