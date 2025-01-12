using System;
using System.Threading.Tasks;
using Cinemachine;
using DBMS.RunningData;
using Enemy.Manager;
using Pickup_System;
using PlayerSystem.PlayerSO;
using PlayerSystem.Services.HealthService;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Signals.InputSignals;
using PlayerSystem.Views;
using Unity.Mathematics;
using UnityEngine;
using WeaponSystem.Managers;
using Zenject;
using Vector2 = UnityEngine.Vector2;

namespace PlayerSystem.Controllers
{
    [Serializable]
    public class PlayerController : IInitializable, IFixedTickable, IDisposable, IPickupCollector
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        
        private readonly RunningDataScriptable _runningDataScriptable;

        private readonly PlayerScriptableObject _playerScriptableObject;

        private readonly PlayerHealthService _playerHealthService;
        
        private readonly PlayerView _playerView;
        private readonly PlayerHealthView _playerHealthView;
        
        private readonly SignalBus _signalBus;
        
        private readonly WeaponManager _weaponManager;
        private EnemyManager _enemyManager;
        
        
        
        #region Player Settings Variables

        private bool _isAutoAttacking;
        private float _speed = 5f;

        #endregion

        #region Dash Variables

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
        
        private PlayerController (CinemachineVirtualCamera cineMachineVirtualCamera,
            RunningDataScriptable runningDataScriptable, 
            WeaponManager weaponManager, 
            SignalBus signalBus,
            PlayerScriptableObject playerScriptableObject,
            PlayerView playerView,
            PlayerHealthView playerHealthView,
            PlayerHealthService playerHealthService)
        {
            _cineMachineVirtualCamera = cineMachineVirtualCamera;
            
            _runningDataScriptable = runningDataScriptable;
            _playerScriptableObject = playerScriptableObject;
            
            _weaponManager = weaponManager;
            
            _signalBus = signalBus;
            
            _playerView = playerView;
            _playerHealthView = playerHealthView;

            _playerHealthService = playerHealthService;
            
            _cineMachineVirtualCamera.Follow = playerView.transform;
        }
        
        public void Initialize()
        {
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
            
            SetHealthAndShield();
            _signalBus.Fire(new HeavyAttackChargeMeterSignal(3, 3));   // setting initial height to 4
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<DashStartInputSignal>(StartDash);
            _signalBus.Subscribe<DashStopInputSignal>(() => { });
            _signalBus.Subscribe<ToggleAutoAttackInputSignal>(ToggleAutoAttack);
            _signalBus.Subscribe<PlayerMovementSignal>(Move);
            _signalBus.Subscribe<LightAttackInputSignal>(Attack);
            _signalBus.Subscribe<StartHeavyAttackInputSignal>(CheckHeavyAttackEligibility);
            _signalBus.Subscribe<StopHeavyAttackInputSignal>(StopHeavyAttack);
            _signalBus.Subscribe<CancelHeavyAttackSignal>(CancelHeavyAttackWithDash);
            _signalBus.Subscribe<WeaponThrowStartInputSignal>(CheckWeaponThrowEligibility);
            _signalBus.Subscribe<WeaponThrowStopInputSignal>(StopWeaponThrow);
        }
        
        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<DashStartInputSignal>(StartDash);
            _signalBus.Unsubscribe<ToggleAutoAttackInputSignal>(ToggleAutoAttack);
            _signalBus.Unsubscribe<PlayerMovementSignal>(Move);
            _signalBus.Unsubscribe<LightAttackInputSignal>(Attack);
            _signalBus.Unsubscribe<StartHeavyAttackInputSignal>(CheckHeavyAttackEligibility);
            _signalBus.Unsubscribe<StopHeavyAttackInputSignal>(StopHeavyAttack);
            _signalBus.Unsubscribe<CancelHeavyAttackSignal>(CancelHeavyAttackWithDash);
        }

        #endregion
        
        #endregion

        private void SetHealthAndShield()
        {
            _playerHealthService.SetHealth(_playerScriptableObject.player, _playerScriptableObject.player.maxHealth);
            _playerHealthService.SetShield(_playerScriptableObject.player, _playerScriptableObject.player.maxShield);
            
            _playerHealthView.maxHealthBar = _playerScriptableObject.player.maxHealth;
            _playerHealthView.maxShieldBar = _playerScriptableObject.player.maxShield;
            _playerHealthView.HealthBar = _playerScriptableObject.player.health;
            _playerHealthView.ShieldBar = _playerScriptableObject.player.shield;
        }
        
        public void FixedTick()
        {
            _runningDataScriptable.playerVelocity = _playerView.rb.linearVelocity;
            _runningDataScriptable.playerVelocityMagnitude = _playerView.rb.linearVelocity.magnitude;
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
            if(!canMove) return;
            _playerView.rb.linearVelocity = movement * _speed;
            var stateInfo = _playerView.playerAnimationController.animator.GetCurrentAnimatorStateInfo(0);

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
                {
                    var angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;

                    switch (angle)
                    {
                        case >= -22.5f and < 22.5f:
                            _playerView.playerAnimationController.PlayAnimation("runRight");
                            break;
                        case >= 22.5f and < 67.5f:
                            _playerView.playerAnimationController.PlayAnimation("runUpRight");
                            break;
                        case >= 67.5f and < 112.5f:
                            _playerView.playerAnimationController.PlayAnimation("runUp");
                            break;
                        case >= 112.5f and < 157.5f:
                            _playerView.playerAnimationController.PlayAnimation("runUpLeft");
                            break;
                        case >= 157.5f or < -157.5f:
                            _playerView.playerAnimationController.PlayAnimation("runLeft");
                            break;
                        case >= -157.5f and < -112.5f:
                            _playerView.playerAnimationController.PlayAnimation("runDownLeft");
                            break;
                        case >= -112.5f and < -67.5f:
                            _playerView.playerAnimationController.PlayAnimation("runDown");
                            break;
                        case >= -67.5f and < -22.5f:
                            _playerView.playerAnimationController.PlayAnimation("runDownRight");
                            break;
                    }
                }
            }
            else if (stateInfo.length == 0 || stateInfo.normalizedTime >= 1.0f)
            {
                _playerView.playerAnimationController.PlayAnimation("idle");
            }
        }

        #endregion

        #region Light Attack

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
            if(!isHeavyAttackCharging) return;
            heavyAttackChargeMeterAngle = 0;
            heavyAttackChargeMeterDistance = 0;
            
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
            
            _signalBus.Fire<DashPerformSignal>();
            LungeDash();
        }
        
        private async void LungeDash()
        {
            try
            {
                _isDashing = true;
                _isLungeDashing = true;
                _speed = _lungeDashSpeed;
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
                _isRollDashing = true;
                _speed = _rollDashSpeed;
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

        #region Weapon Throw

        public bool canThrowWeapon = true; 
        private void CheckWeaponThrowEligibility()
        {
            if(!canThrowWeapon) return;
            InitiateWeaponThrow();
        }

        private void InitiateWeaponThrow()
        {
            var weaponThrowEligible = _weaponManager.CheckEquippedWeaponThrowEligibility();
            
                
        }

        private void StopWeaponThrow()
        {
            
        }

        #endregion
        
        #region Damage
        
        private bool _canTakeDamage;

        private bool CheckTakeDamageEligibility()
        {
            return true;
        }
        
        public void Damage(float damageAmount)
        {
            _playerHealthService.TakeDamage(_playerScriptableObject, damageAmount);
            _playerHealthView.ShieldBar = _playerScriptableObject.player.shield;
            _playerHealthView.HealthBar = _playerScriptableObject.player.health;
        }

        #endregion

        
        public void Dispose()
        {
            UnsubscribeToActions();
        }

        #region Test

        private float _testDamageValue = 30f;
        public void DamageTest()
        {
            Damage(_testDamageValue);
        }

        #endregion

        public Vector3 Position => _playerView.transform.position;
        public float MagnetRadius => _playerScriptableObject.player.dropCollectionMagnetRadius;
        public float MagnetStrength => _playerScriptableObject.player.dropCollectionMagnetStrength;

        public bool CanCollectPickup(IPickupable pickup)
        {
            switch (pickup)
            {
                case ExpCrystal expCrystal:
                    return true; // Always can collect exp
                
                // case InventoryItem inventoryItem:
                //     // Check if inventory has space
                //     return inventorySystem.HasSpaceFor(inventoryItem.ItemId);
                
                default:
                    Debug.LogWarning($"Unknown pickup type: {pickup.GetType()}");
                    return false;
            }
        }

        public void CollectPickup(IPickupable pickup)
        {
            switch (pickup)
            {
                case ExpCrystal expCrystal:
                    Debug.Log($"Exp Collected from the Crystal{expCrystal.ExpValue}");
                    //expSystem.AddExperience(expCrystal.ExpValue);
                    break;
                
                // case InventoryItem inventoryItem:
                //     inventorySystem.AddItem(inventoryItem.ItemId);
                //     break;
                //
                default:
                    Debug.LogWarning($"Trying to collect unknown pickup type: {pickup.GetType()}");
                    return;
            }
        }
    }
}
