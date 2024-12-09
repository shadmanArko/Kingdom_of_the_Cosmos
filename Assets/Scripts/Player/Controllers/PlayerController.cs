using System;
using System.Threading.Tasks;
using Cinemachine;
using DBMS.RunningData;
using Enemy.Manager;
using Player.Signals.BattleSceneSignals;
using Player.Views;
using Unity.Mathematics;
using UnityEngine;
using WeaponSystem.Managers;
using Zenject;
using Vector2 = UnityEngine.Vector2;

namespace Player.Controllers
{
    [Serializable]
    public class PlayerController : IInitializable, IFixedTickable, IDisposable 
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        private readonly WeaponManager _weaponManager;
        private EnemyManager _enemyManager;
        private readonly RunningDataScriptable _runningDataScriptable;
        private readonly PlayerView _playerView;
        private readonly SignalBus _signalBus;
        
        #region Player Settings Variables

        private bool _isAutoAttacking;
        private float _speed = 5f;

        #endregion

        #region Dash Variables

        private Vector2 _dashDirection;
        
        private int _dashCount = 2;
        private int _totalDashCount = 2;
        
        private bool _isDashing;
        private bool _canDash;
        
        private float _lungeDashSpeed = 8f;
        private float _rollDashSpeed = 5f;
        
        private float _lungeDashDuration = 0.4f;
        private float _rollDashDuration = 0.6f;
        
        private bool _isLungeDashing;
        private bool _isRollDashing;
        
        private float _dashCooldownDuration = 3f;

        #endregion

        #region Move Variables

        public float moveSpeed = 3f;
        public Vector2 movement;
        public bool canMove;

        #endregion

        #region Attack Variables

        public bool canAttack;

        #endregion

        #region Initializers
        
        private PlayerController (CinemachineVirtualCamera cineMachineVirtualCamera,RunningDataScriptable runningDataScriptable, WeaponManager weaponManager, SignalBus signalBus, PlayerView playerView)
        {
            _cineMachineVirtualCamera = cineMachineVirtualCamera;
            _runningDataScriptable = runningDataScriptable;
            _weaponManager = weaponManager;
            _signalBus = signalBus;
            _playerView = playerView;
            
            _cineMachineVirtualCamera.Follow = playerView.transform;
        }
        
        public void Initialize()
        {
            Debug.Log("Initialize from Player Controller");
            SubscribeToActions();
            canMove = true;
            _isDashing = false;
            _canDash = true;
            
            canAttack = true;
            canPerformLightAttack = true;
            canPerformHeavyAttack = true;
            
            _speed = moveSpeed;
            _dashCooldownDuration = 2f;
            _totalDashCount = 2;
            _runningDataScriptable.playerController = this;
            
            _signalBus.Fire(new HeavyAttackChargeMeterSignal(3, 3));   // setting initial height to 4
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<StartDashSignal>(StartDash);
            _signalBus.Subscribe<StopDashSignal>(() => { });
            _signalBus.Subscribe<ToggleAutoAttackSignal>(ToggleAutoAttack);
            _signalBus.Subscribe<PlayerMovementSignal>(Move);
            _signalBus.Subscribe<StartHeavyAttackSignal>(CheckHeavyAttackEligibility);
            _signalBus.Subscribe<StopHeavyAttackSignal>(StopHeavyAttack);
            _signalBus.Subscribe<CancelHeavyAttackSignal>(CancelHeavyAttackWithDash);
        }
        
        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<StartDashSignal>(StartDash);
            _signalBus.Unsubscribe<ToggleAutoAttackSignal>(ToggleAutoAttack);
            _signalBus.Unsubscribe<PlayerMovementSignal>(Move);
            _signalBus.Unsubscribe<StartHeavyAttackSignal>(CheckHeavyAttackEligibility);
            _signalBus.Unsubscribe<StopHeavyAttackSignal>(StopHeavyAttack);
            _signalBus.Unsubscribe<CancelHeavyAttackSignal>(CancelHeavyAttackWithDash);
        }

        #endregion
        
        #endregion
        
        public void FixedTick()
        {
            if (_lightAttackTimer > 0)
                _lightAttackTimer -= Time.fixedDeltaTime;
            else if(_isAutoAttacking) 
                AutoAttack();
            
            if(heavyAttackTimer > 0)
                heavyAttackTimer -= Time.fixedDeltaTime;
            
            if(isHeavyAttackCharging)
                ChargeHeavyAttackMeter();
        }

        #region Player Movement

        private void Move(PlayerMovementSignal playerMovementSignal)
        {
            movement = playerMovementSignal.MovePos.normalized;
            Debug.LogWarning($"direction: {movement.x}, {movement.y}");
            if(!canMove) return;
            _playerView.rb.linearVelocity = movement * _speed;
            // if (!_isDashing)
            // {
            //     // movement = direction.normalized;
            //     _playerView.rb.linearVelocity = movement * _speed;
            // }
            // else
            // {
            //     Debug.Log("Dashing...");
            //     // _dashDirection = _isRollDashing ? movement : _dashDirection;
            //     _playerView.rb.linearVelocity = _dashDirection * _speed;
            // }

            if (_playerView.rb.linearVelocity.magnitude > 0)
            {
                if (_isDashing)
                {
                    if (_isLungeDashing)
                        _playerView.playerAnimationController.PlayAnimation( "dash");
                    else if (_isRollDashing)
                        _playerView.playerAnimationController.PlayAnimation( "roll");
                }
                else
                    _playerView.playerAnimationController.PlayAnimation( "run");
            }
            // else
            // {
            //     var stateInfo = _playerView.playerAnimationController.animator.GetCurrentAnimatorStateInfo(0);
            //     if (stateInfo.length == 0 || stateInfo.normalizedTime >= 1.0f)
            //     {
            //         _playerView.playerAnimationController.PlayAnimation("idle");
            //     }
            // }
        }

        #endregion

        #region Attack

        public bool canPerformLightAttack;
        private readonly float _lightAttackCooldownTimer = 1f;
        private float _lightAttackTimer;

        
        private void Attack()
        {
            if(!canAttack) return;
            if(_lightAttackTimer > 0) return;
            if (!_weaponManager.TriggerControlledWeaponLightAttack()) return;
            _playerView.playerAnimationController.PlayAnimation("attack");  
            _lightAttackTimer = _lightAttackCooldownTimer;
        }

        #endregion

        #region Heavy Attack

        public bool canPerformHeavyAttack = true;
        public bool isHeavyAttackCharging;
        
        public float heavyAttackChargeMeterDistance = 0.1f;
        public float heavyAttackChargeMeterDistanceThreshold = 0.5f;
        public const float HeavyAttackChargeMeterDistanceLimit = 3f;
        
        public float heavyAttackChargeMeterAngle = 0.1f;
        public float heavyAttackChargeMeterAngleThreshold = 2f;
        public const float HeavyAttackChargeMeterAngleLimit = 10f;
        
        
        public float heavyAttackTimer;
        public const float HeavyAttackCooldownTimer = 5f;
        private void CheckHeavyAttackEligibility()
        {
            if(!canAttack) return;
            if(!canPerformHeavyAttack) return;
            if(isHeavyAttackCharging) return;
            if(heavyAttackTimer > 0) return;
            InitiateHeavyAttackCharge();
        }

        private void InitiateHeavyAttackCharge()
        {
            canMove = false;
            canPerformHeavyAttack = false;
            heavyAttackChargeMeterDistance = 0f;
            _playerView.rb.linearVelocity = Vector2.zero;
            isHeavyAttackCharging = true;
        }

        private void ChargeHeavyAttackMeter()
        {
            if (heavyAttackChargeMeterDistance < HeavyAttackChargeMeterDistanceLimit)
            {
                heavyAttackChargeMeterDistance += Time.fixedDeltaTime;
                heavyAttackChargeMeterDistance = Mathf.Clamp(heavyAttackChargeMeterDistance, 0.1f, HeavyAttackChargeMeterDistanceLimit);
                
                heavyAttackChargeMeterAngle += 0.06f;
                heavyAttackChargeMeterAngle = Mathf.Clamp(heavyAttackChargeMeterAngle, 0.1f, HeavyAttackChargeMeterAngleLimit);
                
                _signalBus.Fire(new HeavyAttackChargeMeterSignal(heavyAttackChargeMeterAngle, heavyAttackChargeMeterDistance * 3)); // meter increasing
                return;
            }
            
            _signalBus.Fire(new HeavyAttackChargeMeterSignal(heavyAttackChargeMeterAngle, HeavyAttackChargeMeterDistanceLimit * 3));   // meter max base 90 degrees
            PerformHeavyAttack();
            StopHeavyAttack();
        }
        
        private void PerformHeavyAttack()
        {
            if(!canAttack) return;
            if (!_weaponManager.TriggerControlledWeaponHeavyAttack()) return;
            _playerView.playerAnimationController.PlayAnimation("attack");
            
            heavyAttackChargeMeterDistance = 0f;
            heavyAttackChargeMeterAngle = 0f;
            heavyAttackTimer = HeavyAttackCooldownTimer;
        }
        
        private void StopHeavyAttack()
        {
            if(!isHeavyAttackCharging) return;
            isHeavyAttackCharging = false;
            
            if(heavyAttackChargeMeterDistance >= heavyAttackChargeMeterDistanceThreshold && 
               heavyAttackChargeMeterAngle >= heavyAttackChargeMeterAngleThreshold) 
                PerformHeavyAttack();
            
            canMove = true;
            heavyAttackChargeMeterDistance = 0f;
            heavyAttackChargeMeterAngle = 0f;
            _signalBus.Fire(new HeavyAttackChargeMeterSignal(3, 3));   // meter back to normal
            canPerformHeavyAttack = true;
        }

        private void CancelHeavyAttackWithDash()
        {
            heavyAttackChargeMeterAngle = 0;
            heavyAttackChargeMeterDistance = 0;
            
            // StopHeavyAttack();
            canMove = true;
            isHeavyAttackCharging = false;
            canPerformHeavyAttack = true;
            _signalBus.Fire(new HeavyAttackChargeMeterSignal(3, 3));   // meter back to normal
            
            heavyAttackTimer = HeavyAttackCooldownTimer / 2f;
        }
        
        #endregion

        #region Auto Attack

        private void ToggleAutoAttack() => _isAutoAttacking = !_isAutoAttacking;

        private void AutoAttack()
        {
            if(!canAttack) return;
            var closestEnemyPosition = _runningDataScriptable.closestEnemyToPlayer;
            var direction = (_playerView.transform.position - closestEnemyPosition).normalized * -1;
            _runningDataScriptable.attackDirection = direction;

            var playerAttackAngleDirection = closestEnemyPosition - _runningDataScriptable.playerAttackAnglePosition;
            playerAttackAngleDirection.z = 0;

            var angle = Mathf.Atan2(playerAttackAngleDirection.y, playerAttackAngleDirection.x) * Mathf.Rad2Deg - 90;
            _runningDataScriptable.attackAngle = angle;
            
            if(_lightAttackTimer <= 0 && canPerformLightAttack) Attack();
        }
        
        #endregion

        #region Dash
        
        private void StartDash()
        {
            if(!canMove) return;
            if(!_canDash) return;
            if(_isDashing) return;
            if(_dashCount <= 0) return;
            Debug.Log("Start Dash called");
            canAttack = false;
            _isDashing = true;
            
            LungeDash();
        }
        
        private async void LungeDash()
        {
            try
            {
                _isDashing = true;
                _dashDirection = movement.normalized;
                _isLungeDashing = true;
                _speed = _lungeDashSpeed;
                Debug.Log($"Lunge Dashing direction: {_dashDirection}");
                await Task.Delay(Mathf.CeilToInt(_lungeDashDuration * 1000));
                _isLungeDashing = false;
            
                RollDash();
            }
            catch (Exception e)
            {
                Debug.LogError($"Lunge Dash Error: {e}");
            }
        }

        private async void RollDash()
        {
            try
            {
                _dashDirection = movement.normalized;
                _isRollDashing = true;
                _speed = _rollDashSpeed;
                Debug.Log($"Roll Dashing direction: {_dashDirection}");
                await Task.Delay(Mathf.CeilToInt(_rollDashDuration * 1000));
                _isRollDashing = false;
            
                StopDash();
                StartDashCooldown();
            }
            catch (Exception e)
            {
                Debug.LogError($"Roll Dash Error: {e}");
            }
        }

        private void StopDash()
        {
            if(!_isDashing) return;
            Debug.Log("Stop Dash called");
            _speed = moveSpeed;
            canAttack = true;
            _isDashing = false;
        }

        private async void StartDashCooldown()
        {
            try
            {
                _dashCount -= 1;
                await Task.Delay(Mathf.CeilToInt(_dashCooldownDuration * 1000));
                _dashCount += 1;
                _dashCount = math.clamp(_dashCount, 0, _totalDashCount);
            }
            catch (Exception e)
            {
                Debug.Log($"Start Dash Cooldown Error: {e}");
            }
        }

        #endregion
        
        
        public void Dispose()
        {
            UnsubscribeToActions();
        }
    }
}
