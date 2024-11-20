using System;
using System.Collections.Generic;
using DBMS.RunningData;
using Signals.BattleSceneSignals;
using UnityEngine;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;

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

            _runningDataScriptable.attackAngle = new List<Vector2>();
        }

        private void Start()
        {
            SubscribeToActions();
        }

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<MouseMovementSignal>(DetermineAttackAngle);
        }
        
        private void DetermineAttackAngle()
        {
            var mouseDirection = _runningDataScriptable.attackDirection;
            mouseDirection.Normalize();
            var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            _runningDataScriptable.attackAngle.Clear();
            foreach (var point in points)
                _runningDataScriptable.attackAngle.Add(point.position);

            for (var i = 0; i < points.Count; i++)
            {
                Debug.Log($"p{i}: {points[i].position}");
            }
        }
    }
}