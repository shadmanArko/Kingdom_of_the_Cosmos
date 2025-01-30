using System.Collections;
using System.Threading.Tasks;
using Enemy.Models;
using PlayerSystem;
using PlayerSystem.Views;
using UnityEngine;

namespace Enemy.Services
{
    public class FlyingEnemy : BaseEnemy
    {
        [SerializeField] private PlayerAnimationController _animationController;
        [SerializeField] private RangedAttackPreview _rangedAttackPreview;
        public EnemyProjectilePoolManager enemyProjectilePoolManager;
        private int _numberOfProjectiles = 1;

        public float MinimumDistanceForDash = 1.5f;
        public float DashDistance = 5f;
        public float DashCooldownTime = 10f;
        private float lastDashTime = 0f;
        private bool isDashing = false;
        // [Inject]
        // public void Construct(EnemyProjectilePoolManager enemyProjectilePoolManager)
        // {
        //     _enemyProjectilePoolManager = enemyProjectilePoolManager;
        //     Debug.LogError(_enemyProjectilePoolManager == null
        //         ? "enemyProjectilePoolManager in Null"
        //         : "enemyProjectilePoolManager is not null");
        // }

        protected override void Start()
        {
            base.Start();
            _animationController.PlayAnimation("run");
        }

        

        public override void GetBuff(EnemyBuffTypes buffType, float amount, float duration)
        {
            if (!canGetBuff) return; 
            StartCoroutine(ApplyTemporaryBuff(amount, duration));
        }
        private IEnumerator ApplyTemporaryBuff(float amount, float duration)
        {
            // Apply the buff
            _numberOfProjectiles += (int)amount;
            hasShaman = true;
            canGetBuff = false;

            // Wait for the specified duration
            yield return new WaitForSeconds(duration);

            // Remove the buff
            _numberOfProjectiles -= (int)amount;
            hasShaman = false;
            canGetBuff = true;
        }

        private float wobbleTime = 0f;
        public float verticalWobbleFrequency = 2f;
        public float horizontalWobbleFrequency = 1.5f;
        public float verticalWobbleAmplitude = 0.5f;
        public float horizontalWobbleAmplitude = 0.3f;

        public override void MoveTowardsTarget(Transform targetTransform)
{
    _rigidbody2D.linearVelocity = Vector2.zero;
    if (!canGetBuff || !canMove) return;
     
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

    if (distanceToPlayer < MinimumDistanceForDash && !isAttacking && !isDashing)
    {
        // Check if enough time has passed since last dash
        if (Time.time > lastDashTime + DashCooldownTime)
        {
            // Calculate dash direction AWAY from player
            Vector3 dashDirection = (transform.position - targetTransform.position).normalized;
            
            // Start dash
            _ = PerformDashAsync(dashDirection);
            lastDashTime = Time.time;
        }
    }

    Position = transform.position;

    // Add wobble effect only if not dashing
    if (!isDashing)
    {
        wobbleTime += Time.deltaTime;
        Vector3 wobbleOffset = new Vector3(
            Mathf.Sin(wobbleTime * horizontalWobbleFrequency) * horizontalWobbleAmplitude,
            Mathf.Sin(wobbleTime * verticalWobbleFrequency) * verticalWobbleAmplitude,
            0
        );
        transform.position += wobbleOffset * Time.deltaTime;
    }
}

private async Task PerformDashAsync(Vector3 direction)
{
    isDashing = true;
    
    // Store initial position
    Vector3 startPosition = transform.position;
    Vector3 targetPosition = startPosition + direction * DashDistance;
    
    float elapsedTime = 0;
    float dashDuration = 0.7f; // Adjust this value to make dash faster or slower
    
    while (elapsedTime < dashDuration)
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / dashDuration;
        
        // Use smooth interpolation for dash movement
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
        await Task.Yield(); // Wait for next frame
    }
    
    isDashing = false;
}

        public override async void Attack(PlayerView target)
        {
            _animationController.PlayAnimation("attack");
            
            // Calculate the spread angle based on the number of projectiles
            float spreadAngle = CalculateSpreadAngle(_numberOfProjectiles);
            
            // Calculate the original direction to the target
            
            _rangedAttackPreview.StartPreview();
            await Task.Delay(500);
            if (target == null) return;
            
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            for (int i = 0; i < _numberOfProjectiles; i++)
            {
                // Calculate the offset for this specific projectile
                float offsetAngle = CalculateOffsetAngle(spreadAngle, i);
                
                // Rotate the original direction by the offset angle
                Vector3 projectileDirection = Quaternion.Euler(0, 0, offsetAngle) * directionToTarget;
                
                // Create rotation based on the new direction
                Quaternion projectileRotation = Quaternion.LookRotation(Vector3.forward, projectileDirection);
                
                EnemyProjectile projectile = enemyProjectilePoolManager.GetFromPool(transform.position, projectileRotation);
                
                // Calculate target position along the new direction
                Vector3 projectileTarget = transform.position + (projectileDirection * Vector3.Distance(transform.position, target.transform.position));
                
                projectile.SetStats(Damage, 5, target.transform, projectileTarget, enemyProjectilePoolManager);
            }
            
            await Task.Delay(500);
            _animationController.PlayAnimation("run");
            isAttacking = false;
            Debug.Log("Ranged attack occurred");
        }

        // Calculate the spread angle based on the number of projectiles
        private float CalculateSpreadAngle(int projectileCount)
        {
            // Adjust these values to control the spread
            float maxSpreadAngle = 30f; // Maximum spread angle in degrees
            
            // If only one projectile, no spread
            if (projectileCount <= 1)
                return 0f;
            
            // Calculate spread angle, limiting it to maxSpreadAngle
            return Mathf.Min(maxSpreadAngle, maxSpreadAngle / (projectileCount - 1));
        }

        // Calculate the offset angle for a specific projectile
        private float CalculateOffsetAngle(float spreadAngle, int projectileIndex)
        {
            // Center projectiles around the main direction
            int middleIndex = (_numberOfProjectiles - 1) / 2;
            float angle = (projectileIndex - middleIndex) * spreadAngle;
            return angle;
        }
    }
}