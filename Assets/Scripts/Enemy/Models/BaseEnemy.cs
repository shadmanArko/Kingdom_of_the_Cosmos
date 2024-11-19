using System;
using Player;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    protected float maxHealth;
    [SerializeField] protected Slider HealthSlider;

    protected virtual void Start()
    {
        Health = 100;
        maxHealth = 100;
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
        HealthSlider.value = 1 - (maxHealth - Health)/maxHealth;
        Debug.Log($"Took Damage {amount}, health {Health} is alive: {IsAlive}");
    }
    protected virtual void Die()
    {
        IsAlive = false;
    }
}