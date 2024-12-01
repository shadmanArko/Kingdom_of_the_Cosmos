using System;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using DBMS.RunningData;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponSystem.Managers;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        private WeaponManager _weaponManager;
        private EnemyManager _enemyManager;
        private RunningDataScriptable _runningDataScriptable;
        private SignalBus _signalBus;
        
        public float speed = 5f;

        #region Player Settings Variables

        public bool isAutoAttacking;

        #endregion

        #region Dash Variables

        [SerializeField] private Vector2 dashDirection;
        
        [SerializeField] private int dashCount;
        [SerializeField] private int totalDashCount;
        
        [SerializeField] private bool isDashing;
        [SerializeField] private bool canDash;
        
        [SerializeField] private float lungeDashSpeed = 20f;
        [SerializeField] private float rollDashSpeed = 10f;
        
        [SerializeField] private float lungeDashDuration = 0.4f;
        [SerializeField] private float rollDashDuration = 0.4f;
        
        [SerializeField] private bool isLungeDashing;
        [SerializeField] private bool isRollDashing;
        
        [SerializeField] private float dashCooldownDuration;

        #endregion

        #region Move Variables

        public float moveSpeed = 5f;
        [SerializeField] private Vector2 movement;
        [SerializeField] private bool canMove;

        #endregion

        #region Attack Variables

        [FormerlySerializedAs("autoAttackInterval")] [SerializeField] private float attackInterval;
        [FormerlySerializedAs("autoAttackTimer")] [SerializeField] private float attackTimer;

        #endregion
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerAnimationController playerAnimationController;

        #region Initializers
        
        [Inject]
        private void InitializeDiReference(CinemachineVirtualCamera cineMachineVirtualCamera,RunningDataScriptable runningDataScriptable, WeaponManager weaponManager, SignalBus signalBus)
        {
            _cineMachineVirtualCamera = cineMachineVirtualCamera;
            _cineMachineVirtualCamera.Follow = gameObject.transform;
            _runningDataScriptable = runningDataScriptable;
            _weaponManager = weaponManager;
            _signalBus = signalBus;
        }
        
        private void Start()
        {
            SubscribeToActions();
            canMove = true;
            canAttack = true;
            isDashing = false;
            canDash = true;
            speed = moveSpeed;

            dashCooldownDuration = 2f;
            totalDashCount = 2;
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<StartDashSignal>(StartDash);
            _signalBus.Subscribe<StopDashSignal>(() => { });
            _signalBus.Subscribe<ToggleAutoAttackSignal>(ToggleAutoAttack);
        }
        
        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<StartDashSignal>(StartDash);
            _signalBus.Unsubscribe<ToggleAutoAttackSignal>(ToggleAutoAttack);
        }

        #endregion
        
        #endregion
        
        private void FixedUpdate()
        {
            if (attackTimer > 0)
                attackTimer -= Time.fixedDeltaTime;
            else if(isAutoAttacking) 
                AutoAttack();
        }
        
        public void Move(Vector2 direction)
        {
            if(!canMove) return;
            movement = direction.normalized;
            
            if (!isDashing)
            {
                movement = direction.normalized;
                rb.velocity = movement * speed;
            }
            else
            {
                Debug.Log("Dashing...");
                dashDirection = isRollDashing ? direction.normalized : dashDirection;
                rb.velocity = dashDirection * speed;
            }

            if (rb.velocity.magnitude > 0)
            {
                if (isDashing)
                {
                    if (isLungeDashing)
                        playerAnimationController.PlayAnimation( "dash");
                    else if (isRollDashing)
                        playerAnimationController.PlayAnimation( "roll");
                }
                else
                    playerAnimationController.PlayAnimation( "run");
            }
            // else
            // {
            //     var stateInfo = playerAnimationController.animator.GetCurrentAnimatorStateInfo(0);
            //     if (stateInfo.length == 0 || stateInfo.normalizedTime >= 1.0f)
            //     {
            //         playerAnimationController.PlayAnimation("idle");
            //     }
            // }
        }

        #region Attack

        [SerializeField] private bool canAttack;
        public void Attack(Vector2 direction)
        {
            if(!canAttack) return;
            if(attackTimer > 0) return;
            Debug.Log($"Attacking direction: {direction}");
            if (!_weaponManager.TriggerControlledWeapon()) return;
            playerAnimationController.PlayAnimation("attack");
            attackTimer = attackInterval;
        }

        #endregion

        #region Auto Attack

        private void ToggleAutoAttack() => isAutoAttacking = !isAutoAttacking;

        private void AutoAttack()
        {
            if(attackTimer > 0) return;
            //TODO: GET CLOSEST ENEMY AND DIRECT THE
            var closestEnemyTransform = _runningDataScriptable.closestEnemyToPlayer;
            var direction = (closestEnemyTransform != null ? transform.position - closestEnemyTransform.position : Vector3.zero).normalized * -1;
            _runningDataScriptable.attackDirection = direction;
            Attack(direction);
            Debug.Log("Playing auto attack");
        }
        
        #endregion

        #region Dash
        
        private void StartDash()
        {
            if(!canDash) return;
            if(isDashing) return;
            if(dashCount <= 0) return;
            Debug.Log("Start Dash called");
            canAttack = false;
            isDashing = true;
            
            LungeDash();
        }
        
        private async void LungeDash()
        {
            try
            {
                isDashing = true;
                dashDirection = movement.normalized;
                isLungeDashing = true;
                speed = lungeDashSpeed;
                Debug.Log($"Lunge Dashing direction: {dashDirection}");
                await Task.Delay(Mathf.CeilToInt(lungeDashDuration * 1000));
                isLungeDashing = false;
            
                RollDash();
            }
            catch (Exception e)
            {
                Debug.Log($"Lunge Dash Error: {e}");
            }
        }

        private async void RollDash()
        {
            try
            {
                dashDirection = movement.normalized;
                isRollDashing = true;
                speed = rollDashSpeed;
                Debug.Log($"Roll Dashing direction: {dashDirection}");
                await Task.Delay(Mathf.CeilToInt(rollDashDuration * 1000));
                isRollDashing = false;
            
                StopDash();
                StartDashCooldown();
            }
            catch (Exception e)
            {
                Debug.Log($"Roll Dash Error: {e}");
            }
        }

        private void StopDash()
        {
            if(!isDashing) return;
            Debug.Log("Stop Dash called");
            speed = moveSpeed;
            canAttack = true;
            isDashing = false;
        }

        private async void StartDashCooldown()
        {
            try
            {
                dashCount -= 1;
                await Task.Delay(Mathf.CeilToInt(dashCooldownDuration * 1000));
                dashCount += 1;
                dashCount = math.clamp(dashCount, 0, totalDashCount);
            }
            catch (Exception e)
            {
                Debug.Log($"Start Dash Cooldown Error: {e}");
            }
        }

        #endregion

        private void OnDestroy()
        {
            UnsubscribeToActions();
        }
    }
}
