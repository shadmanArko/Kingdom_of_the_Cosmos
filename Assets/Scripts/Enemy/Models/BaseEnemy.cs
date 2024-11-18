using System;
using Player;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseEnemy: MonoBehaviour, IEnemy 
{
    [Header("Basic Enemy Stats")]
    public float2 Position;
    public float2 Velocity;
    public float Health;
    public float Stuckness;
    public float Damage;
    public float DistanceToPlayer;
    public bool IsAlive;

    protected virtual void Start()
    {
        IsAlive = true;
        Health = 100;
        Stuckness = 1;
        Damage = 15;
        DistanceToPlayer = 999f;
        
    }

    public virtual void Move(Vector2 targetPosition)
    {
        transform.position = targetPosition;
    }
    public virtual void SetStat(EnemyData data)
    {
        Position = data.position;
        Velocity = data.velocity;
        Stuckness = data.stuckness;
        Damage = data.damage;
        DistanceToPlayer = data.distanceToPlayer;
        Health = data.health;
        IsAlive = data.isAlive == 1;
    }
    public abstract void Attack(PlayerController target);
    

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        IsAlive = false;
    }
}