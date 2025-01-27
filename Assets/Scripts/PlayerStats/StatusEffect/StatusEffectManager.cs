using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public class StatusEffectManager : MonoBehaviour
    {
        private readonly Dictionary<Type, IStatusEffect> _activeEffects = new();
        private readonly CompositeDisposable _disposables = new();

        public void ApplyEffect<T>(GameObject target, T effect) where T : IStatusEffect
        {
            Type effectType = typeof(T);
        
            // Remove existing effect of the same type
            if (_activeEffects.ContainsKey(effectType))
            {
                _activeEffects[effectType].Remove(target);
            }
        
            _activeEffects[effectType] = effect;
            effect.Apply(target);
        }

        public bool HasEffect<T>() where T : IStatusEffect
        {
            return _activeEffects.ContainsKey(typeof(T)) && _activeEffects[typeof(T)].IsActive;
        }

        private void OnDestroy()
        {
            _disposables.Clear();
        }
        
    }
}