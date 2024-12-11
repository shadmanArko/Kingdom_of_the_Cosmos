using System;
using DBMS.RunningData;
using Player.Signals.BattleSceneSignals;
using Player.Views;
using UnityEngine;
using WeaponSystem.Services.Interfaces;
using Zenject;

namespace Player.Services
{
    public class WeaponThrowService : IInitializable, IFixedTickable, IDisposable
    {
        private readonly RunningDataScriptable _runningDataScriptable;
        private readonly PlayerView _playerView;
        private readonly SignalBus _signalBus;

        private bool _isWeaponThrowCharging;
        private bool _isThrowingWeapon;
        private bool _isPerformingWeaponThrow;

        private float _throwDistance = 1f;
        private float _throwDistanceThreshold = 3f;
        private readonly float _throwDistanceLimit = 15f;

        private ThrowableWeaponView _throwableWeaponView;
        private LineRenderer _lineRenderer;

        private Vector2 startPos;
        private Vector2 endPos;

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
            _signalBus.Subscribe<WeaponThrowStopSignal>(StopWeaponThrow);
        }

        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<WeaponThrowStopSignal>(StopWeaponThrow);
        }

        #endregion

        public void Initialize()
        {
            SubscribeToActions();
            _throwableWeaponView.transform.parent = _playerView.transform;

            _lineRenderer = _throwableWeaponView.gameObject.AddComponent<LineRenderer>();
            SetupLineRenderer();

            _isThrowingWeapon = false;
            _isWeaponThrowCharging = false;
        }
        
        private void SetupLineRenderer()
        {
            _lineRenderer.startWidth = 0.2f;
            _lineRenderer.endWidth = 0.2f;
            _lineRenderer.positionCount = 2;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.blue;
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
            Debug.LogWarning("Weapon Throw is called");
            if(_isThrowingWeapon) return;
            if(_isWeaponThrowCharging) return;
            
            _isThrowingWeapon = true;
            _isWeaponThrowCharging = true;
            _lineRenderer.enabled = true;
        }

        private void ChargeWeaponThrow()
        {
            var throwDirection = _runningDataScriptable.attackDirection;
            if (_throwDistance <= _throwDistanceLimit)
            {
                _throwDistance += Time.deltaTime;
                _throwDistance = Mathf.Clamp(_throwDistance, 1f, _throwDistanceLimit);
                UpdateThrowLine(throwDirection);
                return;
            }
            
            PerformWeaponThrow();
        }
        
        private void PerformWeaponThrow()
        {
            //TODO: throw weapon to endPos
            _isWeaponThrowCharging = false;
            _lineRenderer.enabled = false;
            _isPerformingWeaponThrow = true;
        }

        private void StopWeaponThrow()
        {
            if(!_isThrowingWeapon) return;
            if(!_isWeaponThrowCharging) return;
            if (_throwDistance > _throwDistanceThreshold)
            {
                PerformWeaponThrow();
                return;
            }
            
            _throwDistance = 1f;
            endPos = startPos;
            
            _isWeaponThrowCharging = false;
            _lineRenderer.enabled = false;
            _isThrowingWeapon = false;
        }
        
        private void UpdateThrowLine(Vector2 throwDirection)
        {
            startPos = _playerView.rb.position;
            endPos = startPos + throwDirection.normalized * _throwDistance;

            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);
        }
        
        private float currentSpeed = 0f;
        private float acceleration = 10f;
        private float maxSpeed = 20f;
        
        private void MoveWeaponToEndPosition()
        {
            float distanceToTarget = Vector2.Distance(_throwableWeaponView.transform.position, endPos);
        
            if (distanceToTarget > 0.1f)  // Still need to move
            {
                // Increase speed
                currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
            
                // Calculate direction and move
                var throwableWeaponPos = _throwableWeaponView.transform.position;
                var direction = (new Vector3(endPos.x, endPos.y, 0) - throwableWeaponPos).normalized;
                var newPos = direction * currentSpeed * Time.deltaTime;
                _throwableWeaponView.transform.position += newPos;
            }
            else  // Reached target
            {
                _throwableWeaponView.transform.position = endPos; // Snap to final position
                currentSpeed = 0f;
                
                _isPerformingWeaponThrow = false;
                _throwDistance = 1f;
                StopWeaponThrow();
            }
        }
    }
}