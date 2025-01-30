using System;
using System.Collections.Generic;
using System.Linq;
using PlayerSystem.Models;
using PlayerSystem.PlayerSO;
using UniRx;
using UnityEngine;

namespace PlayerStats
{
    public class PlayerMod
    {
        private readonly PlayerScriptableObject _basePlayer;
        
        private Dictionary<string, float> _baseValues = new Dictionary<string, float>();
        // Reactive properties for each stat
        private Dictionary<string, ReactiveProperty<float>> _reactiveStats 
            = new Dictionary<string, ReactiveProperty<float>>();

        // Temporary modification tracking
        private Dictionary<string, List<TemporaryModification>> _temporaryModifications 
            = new Dictionary<string, List<TemporaryModification>>();
        
        public PlayerScriptableObject BasePlayer => _basePlayer;

        public PlayerMod(PlayerScriptableObject basePlayer)
        {
            _basePlayer = basePlayer;
            InitializeReactiveStats();
            
            
        }

        private void InitializeReactiveStats()
        {
            var playerType = typeof(Player);
            var properties = playerType.GetFields()
                .Where(f => f.FieldType == typeof(float) || f.FieldType == typeof(int));

            foreach (var prop in properties)
            {
                var value = prop.GetValue(_basePlayer.player);
                var initialValue = prop.FieldType == typeof(int) 
                    ? Convert.ToSingle((int)value) 
                    : (float)value;
            
                _baseValues[prop.Name] = initialValue;
                var reactiveProp = new ReactiveProperty<float>(initialValue);
        
                reactiveProp.Subscribe(newValue => {
                    if (prop.FieldType == typeof(int))
                    {
                        prop.SetValue(_basePlayer.player, Mathf.RoundToInt(newValue));
                    }
                    else
                    {
                        prop.SetValue(_basePlayer.player, newValue);
                    }
                });
        
                _reactiveStats[prop.Name] = reactiveProp;
            }
        }

        // Permanent Modification
        public void ModifyPermanentStat(string statName, float value)
        {
            if (_reactiveStats.TryGetValue(statName, out var stat))
            {
                _baseValues[statName] += value;
                stat.Value = _baseValues[statName] + 
                             (_temporaryModifications.ContainsKey(statName) 
                                 ? _temporaryModifications[statName].Sum(mod => mod.Value) 
                                 : 0);
            }
        }

        // Temporary Modification
        public void ApplyTemporaryStat(string statName, float value, float duration)
        {
            if (!_reactiveStats.ContainsKey(statName)) return;

            var modification = new TemporaryModification
            {
                Value = value,
                ExpirationTime = Time.time + duration
            };

            if (!_temporaryModifications.ContainsKey(statName))
            {
                _temporaryModifications[statName] = new List<TemporaryModification>();
            }

            _temporaryModifications[statName].Add(modification);
    
            if (_reactiveStats.TryGetValue(statName, out var stat))
            {
                stat.Value = _baseValues[statName] + 
                             _temporaryModifications[statName].Sum(mod => mod.Value);
            }
        }

        // Update and Clean Temporary Modifications
        public void UpdateTemporaryModifications()
        {
            var currentTime = Time.time;

            foreach (var statModifications in _temporaryModifications)
            {
                // Remove expired modifications
                statModifications.Value.RemoveAll(mod => mod.ExpirationTime <= currentTime);

                // Revert total temporary modifications if all have expired
                if (statModifications.Value.Count == 0)
                {
                    RevertTemporaryModification(statModifications.Key);
                }
            }
        }

        private void RevertTemporaryModification(string statName)
        {
            if (!_temporaryModifications.ContainsKey(statName)) return;

            if (_reactiveStats.TryGetValue(statName, out var stat))
            {
                stat.Value = _baseValues[statName];
            }

            _temporaryModifications[statName].Clear();
        }

        // Get Reactive Property for a specific stat
        public IReadOnlyReactiveProperty<float> GetStatObservable(string statName)
        {
            return _reactiveStats.TryGetValue(statName, out var stat) 
                ? stat 
                : null;
        }

        // Synchronize with Base Player
        public void SyncWithBasePlayer()
        {
            var playerType = typeof(Player);
            var floatProperties = playerType.GetFields()
                .Where(f => f.FieldType == typeof(float) || f.FieldType == typeof(int));

            foreach (var prop in floatProperties)
            {
                if (_reactiveStats.TryGetValue(prop.Name, out var reactiveStat))
                {
                    prop.SetValue(_basePlayer.player, reactiveStat.Value);
                }
            }
        }

        private class TemporaryModification
        {
            public float Value { get; set; }
            public float ExpirationTime { get; set; }
        }
    }
}