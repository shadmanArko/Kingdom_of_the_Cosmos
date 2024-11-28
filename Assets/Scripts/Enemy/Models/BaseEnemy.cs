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
    public Vector2 Position;
    public float2 Velocity;
    public float MoveSpeed;
    public float AttackSpeed;
    public float MaxHealth;
    public float Stuckness;
    public float Damage;
    public float AttackRange;
    public float DistanceToPlayer;
    public float MinDistanceToPlayer;
    public bool IsAlive;
    protected float health;
    [SerializeField] protected Slider HealthSlider;
    protected Rigidbody2D _rigidbody2D;
    protected bool isAttacking;
    protected float lastAttackTime;

    protected virtual void Start()
    {
        
    }

    public virtual void Move(Vector2 targetPosition)
    {
        transform.position = targetPosition;
    }

    public virtual void MoveTowardsTarget(Transform targetTransform)
    {
        _rigidbody2D.velocity = Vector2.zero;
        var distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
        DistanceToPlayer = distanceToPlayer;

        if (distanceToPlayer > MinDistanceToPlayer)
        {
            // Move towards player if too far
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, MoveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer < MinDistanceToPlayer && !isAttacking && Time.time > lastAttackTime + AttackSpeed)
        {
            lastAttackTime = Time.time;
            Attack(targetTransform.GetComponent<PlayerController>());
            isAttacking = true;
        }
        else if (distanceToPlayer < MinDistanceToPlayer && !isAttacking)
        {
            // Backtrack when player is too close
            Vector3 backtrackDirection = transform.position - targetTransform.position;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + backtrackDirection, MoveSpeed * Time.deltaTime);
        }

        Position = transform.position;
    }

    public virtual void Initialize()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        health = MaxHealth;
        HealthSlider.value = 1;
        MoveSpeed = 3f;
        IsAlive = true;
        AttackSpeed = 0.5f;
        DistanceToPlayer = 999f;
        Stuckness = 0.5f;
        AttackRange = 2f;
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