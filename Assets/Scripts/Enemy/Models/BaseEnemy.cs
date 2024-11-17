using Player;
using Unity.Mathematics;
using UnityEngine;

public abstract class BaseEnemy: MonoBehaviour, IEnemy 
{
    [Header("Basic Enemy Stats")]
    [SerializeField] public float2 Position;
    [SerializeField] public float2 Velocity;
    [SerializeField] public float Health;
    [SerializeField] public float Stuckness;
    [SerializeField] public float Damage;
    [SerializeField] public float DistanceToPlayer;
    [SerializeField] public bool IsAlive;
    
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