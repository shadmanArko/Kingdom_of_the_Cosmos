using System;
using System.Threading.Tasks;
using DBMS.RunningData;
using Player.Signals.BattleSceneSignals;
using Player.Views;
using Unity.Mathematics;
using UnityEngine;
using WeaponSystem.Services.Interfaces;
using Zenject;

namespace Player.Services
{
    [Serializable]
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
        private readonly float _throwDistanceLimit = 10f;

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
            _runningDataScriptable.weaponThrowService = this;
            _throwableWeaponView.transform.parent = _playerView.transform;

            _lineRenderer = _throwableWeaponView.GetComponent<LineRenderer>();
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
            
            _throwableWeaponView.transform.position = _playerView.transform.position;
            _throwableWeaponView.throwable.transform.rotation = quaternion.identity;
            _isThrowingWeapon = true;
            _isWeaponThrowCharging = true;
            _lineRenderer.enabled = true;
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
            _finalEndPos = endPos;
            _throwDistance = 1f;
        }

        private void StopWeaponThrow()
        {
            if(!_isThrowingWeapon) return;
            if(!_isWeaponThrowCharging) return;
            _isWeaponThrowCharging = false;
            if (_throwDistance > _throwDistanceThreshold)
                PerformWeaponThrow();
            
            _throwDistance = 1f;
            endPos = startPos;
            
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
        
        private float _currentSpeed;
        private const float Acceleration = 20f;
        private const float MaxSpeed = 30f;
        private Vector2 _finalEndPos;
        public float rotationSpeed = 45f;
        private void MoveWeaponToEndPosition()
        {
            var distanceToTarget = Vector2.Distance(_throwableWeaponView.transform.position, _finalEndPos);
        
            if (distanceToTarget > 0.3f)
            {
                _currentSpeed = Mathf.Min(_currentSpeed + Acceleration * Time.fixedDeltaTime, MaxSpeed);
                
                var throwableWeaponPos = _throwableWeaponView.transform.position;
                var direction = (new Vector3(_finalEndPos.x, _finalEndPos.y, 0) - throwableWeaponPos).normalized;
                var newPos = direction * (_currentSpeed * Time.fixedDeltaTime);
                _throwableWeaponView.transform.position += newPos;
                _throwableWeaponView.throwable.transform.Rotate(0f, 0f, rotationSpeed);
            }
            else 
            {
                _throwableWeaponView.transform.position = _finalEndPos;
                _currentSpeed = 0f;
                
                _isPerformingWeaponThrow = false;
                _throwDistance = 1f;
                StopWeaponThrow();
            }
        }
    }
}