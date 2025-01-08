using System.Collections;
using System.Threading.Tasks;
using Enemy.Models;
using PlayerSystem;
using PlayerSystem.Views;
using UnityEngine;

namespace Enemy.Services
{
    public class MeleeEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;
        private bool _attacking = false;
        [SerializeField] private GameObject warningIndicator; // Assign your sprite GameObject in inspector
        [SerializeField] private float warningDuration = 0.5f;
        [SerializeField] private float warningRadius = 2.0f;
    
        private bool isWarningActive;
        protected override void Start()
        {
            base.Start();
            _animationController.PlayAnimation("run");
        }

        public override void MoveTowardsTarget(Transform targetTransform)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            if (!canMove) return;
            var distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
            DistanceToPlayer = distanceToPlayer;

            if (hasShaman)
            {
                // Circling behavior when shaman buff is active
                float circleRadius = MinDistanceToPlayer + 1f; // Slightly larger than attack range
                Vector3 directionToPlayer = (targetTransform.position - transform.position).normalized;
                Vector3 perpendicular = Vector3.Cross(directionToPlayer, Vector3.forward).normalized;

                // Calculate a point on the circle around the player
                Vector3 circlingPoint = targetTransform.position + perpendicular * circleRadius;

                // Move towards the circling point
                transform.position = Vector3.MoveTowards(transform.position, circlingPoint, MoveSpeed * Time.deltaTime);

                // Check for attack when close enough
                if (distanceToPlayer <= MinDistanceToPlayer && !isAttacking && (Time.time > lastAttackTime + AttackSpeed))
                {
                    lastAttackTime = Time.time;
                    Attack(targetTransform.GetComponent<PlayerView>());
                    isAttacking = true;
                }
            }
            else
            {
                // Original movement logic
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
            }

            Position = transform.position;
        }

        public override void GetBuff(EnemyBuffTypes buffType, float amount, float duration)
        {
            base.GetBuff(buffType, amount, duration);
            if (!canGetBuff) return; // Prevent stacking buffs

            StartCoroutine(ApplyTemporaryBuff(amount, duration));
        }
        private IEnumerator ApplyTemporaryBuff(float amount, float duration)
        {
            // Apply the buff
            MoveSpeed += amount;
            hasShaman = true;
            canGetBuff = false;

            // Wait for the specified duration
            yield return new WaitForSeconds(duration);

            // Remove the buff
            MoveSpeed -= amount;
            hasShaman = false;
            canGetBuff = true;
        }
        public override async void Attack(PlayerView target)
        {
            if (_attacking) return;
            _attacking = true;
            isWarningActive = true;
            canMove = false;
            // Show warning indicator at target position
            Vector3 attackPosition = target.transform.position;
            var warningIndicatorObj = Instantiate(warningIndicator);
            warningIndicatorObj.transform.position = attackPosition;
            warningIndicatorObj.SetActive(true);
        
            // Wait for warning duration
            await Task.Delay((int)(warningDuration * 1000));
            _animationController.PlayAnimation("attack");
            warningIndicatorObj.SetActive(false);
            await Task.Delay(1000);
            // // Check if player is still in attack range
            // if (isWarningActive && IsPlayerInWarningArea(target.transform.position))
            // {
            //     
            // }
        
            // Hide warning indicator and reset state
            Destroy(warningIndicatorObj);
            _attacking = false;
            isAttacking = false;
            isWarningActive = false;
            canMove = true;
            _animationController.PlayAnimation("run");
        }
    
        private bool IsPlayerInWarningArea(Vector3 playerPosition)
        {
            Vector3 warningCenter = warningIndicator.transform.position;
            float distance = Vector2.Distance(
                new Vector2(warningCenter.x, warningCenter.z),
                new Vector2(playerPosition.x, playerPosition.z)
            );
            return distance <= warningRadius;
        }

        public void CancelAttack()
        {
            isWarningActive = false;
            warningIndicator.SetActive(false);
        }
    }
}