using System.Collections;
using Enemy.Services;
using Player;
using Player.Controllers;
using Player.Views;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy.Models
{
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
        protected bool canMove = true;
        protected float lastAttackTime;
        protected bool canGetBuff = true;
        protected bool hasShaman = false;
        protected virtual void Start()
        {
        
        }

        public virtual void Move(Vector2 targetPosition)
        {
            transform.position = targetPosition;
        }

        public virtual void MoveTowardsTarget(Transform targetTransform)
        {
             if (!canGetBuff) return;
             
            _rigidbody2D.linearVelocity = Vector2.zero;
            var distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
            DistanceToPlayer = distanceToPlayer;

            if (distanceToPlayer > MinDistanceToPlayer)
            {
                // Move towards player if too far
                transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, MoveSpeed * Time.deltaTime);
            }
            else if (distanceToPlayer <= MinDistanceToPlayer && !isAttacking && (Time.time > lastAttackTime + AttackSpeed))
            {
                lastAttackTime = Time.time;
                Attack(targetTransform.GetComponent<PlayerView>());
                isAttacking = true;
            }
        
            if (distanceToPlayer < MinDistanceToPlayer && !isAttacking)
            {
                // Backtrack when player is too close
                Vector3 backtrackDirection = transform.position - targetTransform.position;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + backtrackDirection, MoveSpeed * Time.deltaTime);
                Debug.DrawRay(transform.position, targetTransform.position, Color.red);
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
        
            AttackSpeed = 2f;
            DistanceToPlayer = 999f;
            Stuckness = 0.5f;
            AttackRange = 4f;
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
        public abstract void Attack(PlayerView target);
    

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

        public void TakeKnockBack(Transform fromTransform, float duration = 0.2f, float strength = 10f)
        {
            canMove = false;
            Vector3 direction = (transform.position - fromTransform.position).normalized;
            StartCoroutine(ApplyKnockBack(direction, strength, duration));
            
        }
        private IEnumerator ApplyKnockBack(Vector3 direction, float strength, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.position += direction * (strength * Time.deltaTime);
                yield return null;
            }

            canMove = true;
        }
        public virtual void GetBuff(EnemyBuffTypes buffType, float amount, float duration)
        {
            if (!canGetBuff) return; // Prevent stacking buffs
            
        }
        

        protected virtual void Die()
        {
            IsAlive = false;
        }
    }
}