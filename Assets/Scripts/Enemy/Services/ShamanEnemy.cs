using System.Collections;
using UnityEngine;
using Enemy.Models;
using Player;
using Player.Views;

namespace Enemy.Services
{
    public class ShamanEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController playerAnimationController;
        public float shamanRadius = 5f; // Radius of buff effect
        public float shamanInterval = 5f; // Cooldown between buffs
        
        private float lastBuffTime;

        protected override void Start()
        {
            base.Start();
            playerAnimationController.PlayAnimation("run");
        }

        public override void Attack(PlayerView target)
        {
            isAttacking = false;
        }

        public override void MoveTowardsTarget(Transform targetTransform)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            var distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
            DistanceToPlayer = distanceToPlayer;

            if (distanceToPlayer > MinDistanceToPlayer)
            {
                // Move towards player if too far
                transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, MoveSpeed * Time.deltaTime);
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

        private void Update()
        {
            // Check if enough time has passed since last buff
            if (Time.time - lastBuffTime >= shamanInterval)
            {
                ApplyShamanBuff();
            }
        }

        private void ApplyShamanBuff()
        {
            // Use Physics2D.OverlapCircleAll for 2D games
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
                transform.position, 
                shamanRadius// Ensure you have an "Enemy" layer
            );

            foreach (Collider2D enemyCollider in nearbyEnemies)
            {
                // Skip self
                if (enemyCollider.gameObject == gameObject) continue;

                // Get the enemy component
                BaseEnemy enemy = enemyCollider.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    // Apply buff logic
                    ApplyBuff(enemy);
                }
            }

            // Update buff timing
            lastBuffTime = Time.time;
        }

        private void ApplyBuff(BaseEnemy enemy)
        {
            Debug.Log($"ApplyBuff to {enemy.name}");
            enemy.GetBuff(EnemyBuffTypes.Movement, 10, 5); 
        }

        // Optional: Visualize buff radius in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shamanRadius);
        }
    }

    
}