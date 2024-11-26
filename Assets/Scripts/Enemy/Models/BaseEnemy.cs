using System;
using Player;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class BaseEnemy: MonoBehaviour, IEnemy 
{
    [Header("Basic Enemy Stats")]
    public float2 Position;
    public float2 Velocity;
    public float MaxHealth;
    public float Stuckness;
    public float Damage;
    public float DistanceToPlayer;
    public float MinDistanceToPlayer;
    public bool IsAlive;
    protected float health;
    [SerializeField] protected Slider HealthSlider;

    protected virtual void Start()
    {
        
    }

    public virtual void Move(Vector2 targetPosition)
    {
        transform.position = targetPosition;
    }
    public virtual void Initialize()
    {
        health = MaxHealth;
        HealthSlider.value = 1;
        IsAlive = true;
        DistanceToPlayer = 999f;
        Stuckness = 0.5f;
        MinDistanceToPlayer = 10f;
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
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
        HealthSlider.value = 1 - (MaxHealth - health)/MaxHealth;
        Debug.Log($"Took Damage {amount}, health {MaxHealth} is alive: {IsAlive}");
    }
    protected virtual void Die()
    {
        IsAlive = false;
    }
}