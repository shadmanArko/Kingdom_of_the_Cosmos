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
    public class PlayerController : IInitializable, IFixedTickable, IDisposable 
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        private WeaponManager _weaponManager;
        private EnemyManager _enemyManager;
        private RunningDataScriptable _runningDataScriptable;
        private PlayerView _playerView;
        private SignalBus _signalBus;
        
        public float speed = 5f;

        #region Player Settings Variables

        public bool isAutoAttacking;

        #endregion

        #region Dash Variables

        private Vector2 _dashDirection;
        
        private int _dashCount = 2;
        private int _totalDashCount = 2;
        
        private bool _isDashing;
        private bool _canDash;
        
        private float _lungeDashSpeed = 15f;
        private float _rollDashSpeed = 10f;
        
        private float _lungeDashDuration = 0.4f;
        private float _rollDashDuration = 0.6f;
        
        private bool _isLungeDashing;
        private bool _isRollDashing;
        
        private float _dashCooldownDuration = 2;

        #endregion

        #region Move Variables

        public float MoveSpeed = 5f;
        private Vector2 _movement;
        private bool _canMove;

        #endregion

        #region Attack Variables

        private readonly float _attackInterval = 1;
        private float _attackTimer;

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
            _canMove = true;
            _canAttack = true;
            _isDashing = false;
            _canDash = true;
            speed = MoveSpeed;

            _dashCooldownDuration = 2f;
            _totalDashCount = 2;
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<StartDashSignal>(StartDash);
            _signalBus.Subscribe<StopDashSignal>(() => { });
            _signalBus.Subscribe<ToggleAutoAttackSignal>(ToggleAutoAttack);
            _signalBus.Subscribe<PlayerMovementSignal>(Move);
        }
        
        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<StartDashSignal>(StartDash);
            _signalBus.Unsubscribe<ToggleAutoAttackSignal>(ToggleAutoAttack);
            _signalBus.Unsubscribe<PlayerMovementSignal>(Move);
        }

        #endregion
        
        #endregion
        
        public void FixedTick()
        {
            if (_attackTimer > 0)
                _attackTimer -= Time.fixedDeltaTime;
            else if(isAutoAttacking) 
                AutoAttack();
        }
        
        private void Move(PlayerMovementSignal playerMovementSignal)
        {
            if(!_canMove) return;
            var direction = playerMovementSignal.MovePos;
            _movement = direction.normalized;
            
            if (!_isDashing)
            {
                _movement = direction.normalized;
                _playerView.rb.linearVelocity = _movement * speed;
            }
            else
            {
                Debug.Log("Dashing...");
                _dashDirection = _isRollDashing ? direction.normalized : _dashDirection;
                _playerView.rb.linearVelocity = _dashDirection * speed;
            }

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

        #region Attack

        private bool _canAttack;
        private void Attack()
        {
            if(!_canAttack) return;
            if(_attackTimer > 0) return;
            if (!_weaponManager.TriggerControlledWeapon()) return;
            _playerView.playerAnimationController.PlayAnimation("attack");
            _attackTimer = _attackInterval;
        }

        #endregion

        #region Auto Attack

        private void ToggleAutoAttack() => isAutoAttacking = !isAutoAttacking;

        private void AutoAttack()
        {
            if(_attackTimer > 0) return;
            //TODO: GET CLOSEST ENEMY AND DIRECT THE
            var closestEnemyPosition = _runningDataScriptable.closestEnemyToPlayer;
            var direction = (_playerView.transform.position - closestEnemyPosition).normalized * -1;
            _runningDataScriptable.attackDirection = direction;

            var playerAttackAngleDirection = closestEnemyPosition - _runningDataScriptable.playerAttackAnglePosition;
            playerAttackAngleDirection.z = 0;

            var angle = Mathf.Atan2(playerAttackAngleDirection.y, playerAttackAngleDirection.x) * Mathf.Rad2Deg - 90;
            _runningDataScriptable.attackAngle = angle;
            Attack();
            Debug.Log("Playing auto attack");
        }
        
        #endregion

        #region Dash
        
        private void StartDash()
        {
            if(!_canDash) return;
            if(_isDashing) return;
            if(_dashCount <= 0) return;
            Debug.Log("Start Dash called");
            _canAttack = false;
            _isDashing = true;
            
            LungeDash();
        }
        
        private async void LungeDash()
        {
            try
            {
                _isDashing = true;
                _dashDirection = _movement.normalized;
                _isLungeDashing = true;
                speed = _lungeDashSpeed;
                Debug.Log($"Lunge Dashing direction: {_dashDirection}");
                await Task.Delay(Mathf.CeilToInt(_lungeDashDuration * 1000));
                _isLungeDashing = false;
            
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
                _dashDirection = _movement.normalized;
                _isRollDashing = true;
                speed = _rollDashSpeed;
                Debug.Log($"Roll Dashing direction: {_dashDirection}");
                await Task.Delay(Mathf.CeilToInt(_rollDashDuration * 1000));
                _isRollDashing = false;
            
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
            if(!_isDashing) return;
            Debug.Log("Stop Dash called");
            speed = MoveSpeed;
            _canAttack = true;
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
