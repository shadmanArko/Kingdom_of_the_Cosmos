using System;
using DBMS.RunningData;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Signals.InputSignals;
using PlayerSystem.Views;
using Unity.Mathematics;
using UnityEngine;
using WeaponSystem.Services.Interfaces;
using Zenject;

namespace PlayerSystem.Services
{
    [Serializable]
    public class WeaponThrowService : IInitializable, IFixedTickable, IDisposable
    {
        private readonly RunningDataScriptable _runningDataScriptable;
        private readonly PlayerView _playerView;
        private readonly SignalBus _signalBus;

        private IWeapon _currentThrowableWeapon;

        private bool _isWeaponThrowCharging;
        private bool _isThrowingWeapon;
        private bool _isPerformingWeaponThrow;

        private float _throwDistance = 1f;
        private float _throwDistanceThreshold = 3f;
        private readonly float _throwDistanceLimit = 10f;

        private ThrowableWeaponView _throwableWeaponView;
        private LineRenderer _lineRenderer;

        private Vector2 _startPos;
        private Vector2 _endPos;

        public WeaponThrowService(RunningDataScriptable runningDataScriptable, ThrowableWeaponView throwableWeaponView, PlayerView playerView, SignalBus signalBus)
        {
            _runningDataScriptable = runningDataScriptable;
            _throwableWeaponView = throwableWeaponView;
            _playerView = playerView;
            _signalBus = signalBus;
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<WeaponThrowStopInputSignal>(StopWeaponThrow);
            _signalBus.Subscribe<DashPerformSignal>(CancelWeaponThrow);
        }

        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<WeaponThrowStopInputSignal>(StopWeaponThrow);
            _signalBus.Unsubscribe<DashPerformSignal>(CancelWeaponThrow);
        }

        #endregion

        public void Initialize()
        {
            SubscribeToActions();
            _runningDataScriptable.weaponThrowService = this;
            _throwableWeaponView.transform.SetParent(_playerView.transform,false);

            _lineRenderer = _throwableWeaponView.GetComponent<LineRenderer>();
            SetupLineRenderer();

            _isThrowingWeapon = false;
            _isWeaponThrowCharging = false;
            _throwableWeaponView.rb.simulated = false;
        }
        
        private void SetupLineRenderer()
        {
            _lineRenderer.startWidth = 0.05f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.positionCount = 2;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = new Color(1,1,1,0.5f);
            _lineRenderer.endColor = new Color(1,1,1,0.5f);
            _lineRenderer.enabled = false;
            
            _lineRenderer.sortingOrder = 10;
        }

        public void FixedTick()
        {
            if (_isWeaponThrowCharging)
                ChargeWeaponThrow();
            
            if(_isPerformingWeaponThrow)
                MoveWeaponToEndPosition();
        }

        public void Dispose()
        {
            UnsubscribeToActions();
        }

        public void StartWeaponThrow(IWeapon weapon)
        {
            if(_isThrowingWeapon) return;
            if(_isWeaponThrowCharging) return;

            _currentThrowableWeapon = weapon;
            _throwableWeaponView.transform.position = _playerView.transform.position;
            _throwableWeaponView.throwable.transform.rotation = quaternion.identity;
            _throwableWeaponView.rb.simulated = false;
            _isThrowingWeapon = true;
            _isWeaponThrowCharging = true;
            // _endPos = _startPos;
            _lineRenderer.enabled = true;
            // _signalBus.Fire<WeaponThrowChargeSignal>();
        }

        private void ChargeWeaponThrow()
        {
            var throwDirection = _runningDataScriptable.attackDirection;
            if (_throwDistance < _throwDistanceLimit)
            {
                _throwDistance += Time.deltaTime * 5;
                _throwDistance = Mathf.Clamp(_throwDistance, 1f, _throwDistanceLimit);
                UpdateThrowLine(throwDirection);
                return;
            }
            
            StopWeaponThrow();
        }
        
        private void PerformWeaponThrow()
        {
            _isWeaponThrowCharging = false;
            _lineRenderer.enabled = false;
            _isPerformingWeaponThrow = true;
            _finalEndPos = _endPos;
            _throwDistance = 1f;
            
            _throwableWeaponView.weaponData = _currentThrowableWeapon.GetWeaponData();
            _throwableWeaponView.isBeingThrown = true;
            _throwableWeaponView.rb.simulated = true;
        }

        private void StopWeaponThrow()
        {
            if(!_isThrowingWeapon) return;
            if(!_isWeaponThrowCharging) return;
            _isWeaponThrowCharging = false;
            if (_throwDistance > _throwDistanceThreshold)
                PerformWeaponThrow();
            
            _throwDistance = 1f;
            _endPos = _startPos;
            
            _lineRenderer.enabled = false;
            _isThrowingWeapon = false;
        }

        private void CancelWeaponThrow()
        {
            if(!_isWeaponThrowCharging) return;
            _weaponThrowSpeed = 0f;
                
            _isPerformingWeaponThrow = false;
            _throwDistance = 1f;
            StopWeaponThrow();
        }

        #region Helper Functions

        private void UpdateThrowLine(Vector2 throwDirection)
        {
            _startPos = _playerView.rb.position;
            _endPos = _startPos + throwDirection.normalized * _throwDistance;

            _lineRenderer.SetPosition(0, _startPos);
            _lineRenderer.SetPosition(1, _endPos);
        }
        
        private float _weaponThrowSpeed;
        private const float Acceleration = 20f;
        private const float MaxSpeed = 50f;
        private Vector2 _finalEndPos;
        public float rotationSpeed = 20f;
        private void MoveWeaponToEndPosition()
        {
            var distanceToTarget = Vector2.Distance(_throwableWeaponView.transform.position, _finalEndPos);
        
            if (distanceToTarget > 0.3f)
            {
                _weaponThrowSpeed = Mathf.Min(_weaponThrowSpeed + Acceleration * Time.fixedDeltaTime, MaxSpeed);

                var transform = _throwableWeaponView.transform;
                var position = transform.position;
                var throwableWeaponPos = position;
                var direction = (new Vector3(_finalEndPos.x, _finalEndPos.y, 0) - throwableWeaponPos).normalized;
                var newPos = direction * (_weaponThrowSpeed * Time.fixedDeltaTime);
                position += newPos;
                transform.position = position;
                _throwableWeaponView.throwable.transform.Rotate(0f, 0f, rotationSpeed);
            }
            else 
            {
                _throwableWeaponView.transform.position = _finalEndPos;
                _weaponThrowSpeed = 0f;
                
                _isPerformingWeaponThrow = false;
                _throwDistance = 1f;
                StopWeaponThrow();
                // _signalBus.Fire<WeaponThrowCompletedSignal>();
                _throwableWeaponView.rb.simulated = true;
                _throwableWeaponView.isBeingThrown = false;
            }
        }

        #endregion
    }
}