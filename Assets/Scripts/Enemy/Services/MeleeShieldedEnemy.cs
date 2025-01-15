using System.Collections;
using System.Threading.Tasks;
using Enemy.Manager;
using Enemy.Models;
using PlayerSystem;
using PlayerSystem.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy.Services
{
    public class MeleeShieldedEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;
        // [SerializeField] private Slider _shieldSlider;
        [SerializeField] private LineRenderer _attackHighlightLine;
        private bool _attacking = false;
        public float maxShieldHealth;
        private float _shieldHealth;
    
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
                base.MoveTowardsTarget(targetTransform);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _shieldHealth = maxShieldHealth;
            // _shieldSlider.gameObject.SetActive(true);
            // _shieldSlider.value = 1;
            AttackRange = 4f;
        }

        public override void TakeDamage(float amount)
        {
            if (_shieldHealth > 0)
            {
                _shieldHealth -= amount;
                // _shieldSlider.value = 1 - (maxShieldHealth - _shieldHealth) / maxShieldHealth;
                damageValueText.ShowDamageAnimation(amount, Color.blue);
                Debug.Log($"Took shield Damage {amount}");
                if (_shieldHealth<=0)
                {
                    // _shieldSlider.gameObject.SetActive(false);
                }
            }
            else
            {
                base.TakeDamage(amount);
            }
        
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
            canMove = false;
            
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = target.transform.position;
            float attackRange = MinDistanceToPlayer;
            float lineSpeed = 0.5f; // Adjust this value to control line movement speed
            float lineProgress = 0f;
            
            _attackHighlightLine.SetPosition(0, startPosition);
            _attackHighlightLine.SetPosition(1, startPosition); // Start both positions at enemy position

            while (lineProgress < 1f)
            {
                // Check if target moved out of range
                float currentDistance = Vector3.Distance(transform.position, target.transform.position);
                if (currentDistance > attackRange)
                {
                    // Cancel attack
                    EndAttack();
                    return;
                }

                // Update line end position
                lineProgress += lineSpeed * Time.deltaTime;
                startPosition = transform.position;
                targetPosition = target.transform.position;
                Vector3 currentEndPosition = Vector3.Lerp(startPosition, targetPosition, lineProgress);
                _attackHighlightLine.SetPosition(0, startPosition);
                _attackHighlightLine.SetPosition(1, currentEndPosition);
                
                await Task.Yield(); // Wait for next frame
            }

            // Line reached the target, perform lunge attack
            // _animationController.PlayAnimation("lunge");
            
            // Calculate lunge movement
            float lungeSpeed = 10f; // Adjust this value to control lunge speed
            float lungeProgress = 0f;
            Vector3 originalPosition = transform.position;

            // Perform lunge movement
            while (lungeProgress < 1f)
            {
                lungeProgress += lungeSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(originalPosition, targetPosition, lungeProgress);
                await Task.Yield();
            }
            EnemyManager.EnemyDamagedPlayer?.Invoke(Damage);

            // Perform attack at end of lunge
            // _animationController.PlayAnimation("attack");
            // await Task.Delay(1000);

            
            // Reset line and states
            EndAttack();
        }

        private void EndAttack()
        {
            _attackHighlightLine.SetPosition(0, Vector3.zero);
            _attackHighlightLine.SetPosition(1, Vector3.zero);
            _attacking = false;
            isAttacking = false;
            canMove = true;
            _animationController.PlayAnimation("run");
        }
    }
}
