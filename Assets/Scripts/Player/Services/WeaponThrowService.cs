using System;
using DBMS.RunningData;
using Player.Views;
using Unity.Mathematics;
using UnityEngine;
using WeaponSystem.Services.Interfaces;
using Zenject;

namespace Player.Services
{
    public class WeaponThrowService : IInitializable, IFixedTickable, IDisposable
    {
        private readonly RunningDataScriptable _runningDataScriptable;
        private readonly PlayerView _playerView;

        private bool _isWeaponThrowCharging;
        private bool _isThrowingWeapon;

        private float _throwDistance = 1f;
        private float _throwDistanceLimit = 30f;

        private GameObject _throwableObject;
        private LineRenderer _lineRenderer;

        public WeaponThrowService(RunningDataScriptable runningDataScriptable, GameObject throwableObject, PlayerView playerView)
        {
            _runningDataScriptable = runningDataScriptable;
            _throwableObject = GameObject.Instantiate(throwableObject, Vector3.zero, quaternion.identity);
            _playerView = playerView;
        }

        public void Initialize()
        {
            _throwableObject.transform.parent = _playerView.transform;

            _lineRenderer = _throwableObject.AddComponent<LineRenderer>();
            SetupLineRenderer();

            _isThrowingWeapon = false;
            _isWeaponThrowCharging = false;
        }
        
        private void SetupLineRenderer()
        {
            _lineRenderer.startWidth = 1f;
            _lineRenderer.endWidth = 1f;
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
            {
                ChargeWeaponThrow();
            }
        }

        public void Dispose()
        {
            
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
            }
            
            PerformWeaponThrow();
        }
        
        private void UpdateThrowLine(Vector2 throwDirection)
        {
            var startPos = _playerView.rb.position;
            var endPos = (startPos + throwDirection).normalized * _throwDistance;
            
            // _lineRenderer.startColor = new Color(1, 0, 0, 1);  // Red (R,G,B,Alpha)
            // _lineRenderer.endColor = new Color(0, 0, 1, 0.5f); // Blue with 50% transparency
            //
            // _lineRenderer.startWidth = 0.1f;  // Width at start point
            // _lineRenderer.endWidth = 0.1f;

            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);
        }

        private void PerformWeaponThrow()
        {
            
        }

        private void StopWeaponThrow()
        {
            
        }
    }
}