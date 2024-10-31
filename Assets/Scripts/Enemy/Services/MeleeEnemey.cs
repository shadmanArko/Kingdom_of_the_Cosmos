using System;
using Player;
using UnityEngine;
using UnityEngine.Pool;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;
    private void Start()
    {
        _animationController.PlayAnimation("run");
    }
}