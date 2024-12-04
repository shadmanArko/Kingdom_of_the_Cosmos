﻿using System.Collections.Generic;
using DBMS.RunningData;
using Player.Signals.BattleSceneSignals;
using UnityEngine;
using Zenject;

namespace Player
{
    public class AttackAngleFinder : MonoBehaviour
    {
        [SerializeField] private List<Transform> points;

        private RunningDataScriptable _runningDataScriptable;
        private SignalBus _signalBus;
        
        [Inject]
        public void InitializeDiReference(RunningDataScriptable runningDataScriptable, SignalBus signalBus)
        {
            _runningDataScriptable = runningDataScriptable;
            _signalBus = signalBus;

            _runningDataScriptable.attackAnglePoints = new List<Vector2>();
        }

        private void Start()
        {
            SubscribeToActions();
        }

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<MouseMovementSignal>(DetermineAttackAngle);
            _signalBus.Subscribe<HeavyAttackAngleIncrementSignal>(ConstructTriangle);
        }
        
        private void DetermineAttackAngle()
        {
            var mouseDirection = _runningDataScriptable.attackDirection;
            mouseDirection.Normalize();
            var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90;
            _runningDataScriptable.attackAnglePoints.Clear();
            foreach (var point in points)
                _runningDataScriptable.attackAnglePoints.Add(point.position);
            _runningDataScriptable.attackAngle = angle;
        }
        
        private void ConstructTriangle(HeavyAttackAngleIncrementSignal heavyAttackAngleIncrementSignal)
        {
            var baseLength = heavyAttackAngleIncrementSignal.attackAngleBase;
            var height = heavyAttackAngleIncrementSignal.attackAngleHeight;
            if (points[0] == null || points[1] == null || points[2] == null)
            {
                Debug.LogError("Please assign all vertex GameObjects!");
                return;
            }
    
            // Get P0's transform information
            Vector3 p0Position = points[0].transform.position;
            Quaternion p0Rotation = points[0].rotation;
    
            // Calculate base center position using P0's forward direction
            Vector3 baseCenter = p0Position + p0Rotation * Vector3.down * height;
    
            // Position base vertices using P0's right direction for the base
            Vector3 rightDirection = p0Rotation * Vector3.right;
            points[1].transform.position = baseCenter - rightDirection * (baseLength / 2f);
            points[2].transform.position = baseCenter + rightDirection * (baseLength / 2f);
        }

        private void Update()
        {
            _runningDataScriptable.playerAttackAnglePosition = transform.position;
            transform.rotation = Quaternion.Euler(0, 0, _runningDataScriptable.attackAngle);
            _runningDataScriptable.attackAnglePoints.Clear();
            foreach (var point in points)
                _runningDataScriptable.attackAnglePoints.Add(point.position);
        }

        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<MouseMovementSignal>(DetermineAttackAngle);
            _signalBus.Unsubscribe<HeavyAttackAngleIncrementSignal>(ConstructTriangle);
        }

        private void OnDestroy()
        {
            UnsubscribeToActions();
        }
    }
}