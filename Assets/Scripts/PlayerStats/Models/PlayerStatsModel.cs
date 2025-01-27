using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayerStats.StatusEffect;
using UniRx;
using UnityEngine;

namespace PlayerStats
{
    public class PlayerStatsModel
    {
        private readonly Dictionary<StatType, ReactiveProperty<float>> _permanentStats = new();
        private readonly Dictionary<StatType, ReactiveProperty<float>> _temporaryStats = new();
        private readonly Dictionary<StatType, IDisposable> _temporaryStatTimers = new();
        
        private readonly ReactiveProperty<TribeType> _currentTribe = new(TribeType.None);
        private readonly ReactiveProperty<WeaponType> _currentWeapon = new(WeaponType.None);
        
        private readonly CompositeDisposable _disposables = new();
        
        // Tribal effects configuration
        private readonly Dictionary<TribeType, TribalEffect> _tribalEffects = new()
        {
            {
                TribeType.Fire, new TribalEffect
                {
                    TribeType = TribeType.Fire,
                    ElementalDamage = 10f,
                    ProcChance = 0.15f,
                    KillEffectRadius = 3f
                }
            },
            {
                TribeType.Water, new TribalEffect
                {
                    TribeType = TribeType.Water,
                    ElementalDamage = 10f,
                    ProcChance = 0.15f,
                    KillEffectRadius = 3f
                }
            },
            {
                TribeType.Nature, new TribalEffect
                {
                    TribeType = TribeType.Nature,
                    ElementalDamage = 10f,
                    ProcChance = 0.15f,
                    KillEffectRadius = 3f
                }
            },
            {
                TribeType.Air, new TribalEffect
                {
                    TribeType = TribeType.Air,
                    ElementalDamage = 10f,
                    ProcChance = 0.15f,
                    KillEffectRadius = 3f
                }
            }
        };
        
        // Weapon upgrades configuration
        private readonly Dictionary<WeaponType, WeaponUpgrade> _weaponUpgrades = new()
        {
            {
                WeaponType.Sword, new WeaponUpgrade
                {
                    WeaponType = WeaponType.Sword,
                    StatModifiers = new List<StatModifier>
                    {
                        new() { StatType = StatType.Damage, Value = 10f, ModifierType = ModifierType.Percentage },
                        new() { StatType = StatType.AttackRange, Value = 15f, ModifierType = ModifierType.Percentage }
                    },
                    SpecialEffect = "Attack Cleave releases a wave of elemental Energy"
                }
            },
            {
                WeaponType.Bow, new WeaponUpgrade
                {
                    WeaponType = WeaponType.Bow,
                    StatModifiers = new List<StatModifier>
                    {
                        new() { StatType = StatType.Damage, Value = 15f, ModifierType = ModifierType.Percentage },
                        new() { StatType = StatType.ProjectilePierce, Value = 2f, ModifierType = ModifierType.Flat }
                    },
                    SpecialEffect = "Elemental Damage Chains to Nearby Enemies"
                }
            },
            // Add other weapon upgrades based on CSV...
        };
        
        

        public PlayerStatsModel()
        {
            InitializeStats();
            SetupTribalEffects();
            SetupWeaponEffects();
        }
        
        private void SetupTribalEffects()
        {
            _currentTribe
                .Subscribe(tribe =>
                {
                    if (tribe != TribeType.None)
                    {
                        var effect = _tribalEffects[tribe];
                        ApplyTribalEffect(effect).Forget();
                    }
                })
                .AddTo(_disposables);
        }

        private void SetupWeaponEffects()
        {
            _currentWeapon
                .Subscribe(weapon =>
                {
                    if (weapon != WeaponType.None)
                    {
                        var upgrade = _weaponUpgrades[weapon];
                        ApplyWeaponUpgrade(upgrade);
                    }
                })
                .AddTo(_disposables);
        }
        
        private async UniTask ApplyTribalEffect(TribalEffect effect)
        {
            // Apply elemental damage bonus
            await ApplyTemporaryStat(StatType.Damage, effect.ElementalDamage, float.MaxValue);
        
            // Setup proc chance monitoring
            Observable.EveryUpdate()
                .Where(_ => UnityEngine.Random.value < effect.ProcChance)
                .Subscribe(_ => ApplyTribalProc(effect))
                .AddTo(_disposables);
        }

        private void ApplyTribalProc(TribalEffect effect)
        {
            // switch (effect.TribeType)
            // {
            //     case TribeType.Fire:
            //         ApplyBurnEffect();
            //         break;
            //     case TribeType.Water:
            //         ApplySlowEffect();
            //         break;
            //     case TribeType.Nature:
            //         ApplyPoisonEffect();
            //         break;
            //     case TribeType.Air:
            //         ApplyKnockbackEffect();
            //         break;
            // }
        }

        private void ApplyWeaponUpgrade(WeaponUpgrade upgrade)
        {
            foreach (var modifier in upgrade.StatModifiers)
            {
                float value = modifier.ModifierType == ModifierType.Percentage
                    ? GetBaseStat(modifier.StatType) * (modifier.Value / 100f)
                    : modifier.Value;
            
                ModifyPermanentStat(modifier.StatType, value);
            }
        }

        private float GetBaseStat(StatType modifierStatType)
        {
            throw new NotImplementedException();
        }

        public void SetTribe(TribeType tribe)
        {
            _currentTribe.Value = tribe;
        }

        public void SetWeapon(WeaponType weapon)
        {
            _currentWeapon.Value = weapon;
        }

        private void InitializeStats()
        {
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                _permanentStats[statType] = new ReactiveProperty<float>(0);
                _temporaryStats[statType] = new ReactiveProperty<float>(0);
            }
        }

        public IReadOnlyReactiveProperty<float> GetStatValue(StatType statType)
        {
            return _permanentStats[statType].CombineLatest(_temporaryStats[statType], (p, t) => p + t)
                .ToReactiveProperty();
        }

        public void ModifyPermanentStat(StatType statType, float value)
        {
            _permanentStats[statType].Value += value;
        }

        public async UniTask ApplyTemporaryStat(StatType statType, float value, float duration)
        {
            // Cancel existing timer if any
            if (_temporaryStatTimers.ContainsKey(statType))
            {
                _temporaryStatTimers[statType].Dispose();
            }

            _temporaryStats[statType].Value += value;

            // Create new timer
            var cancellationToken = new System.Threading.CancellationTokenSource();
            _temporaryStatTimers[statType] = cancellationToken;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken.Token);
                _temporaryStats[statType].Value -= value;
            }
            catch (OperationCanceledException)
            {
                // Timer was cancelled
            }
        }

        public void RemoveTemporaryStat(StatType statType, float value)
        {
            _temporaryStats[statType].Value = Mathf.Max(0, _temporaryStats[statType].Value - value);
        }
        
        private void ApplyTribalProc(TribalEffect effect, GameObject target)
        {
            if (!target.TryGetComponent<StatusEffectManager>(out var statusManager))
            {
                statusManager = target.AddComponent<StatusEffectManager>();
            }

            switch (effect.TribeType)
            {
                case TribeType.Fire:
                    statusManager.ApplyEffect(target, new BurnEffect());
                    break;
                case TribeType.Water:
                    statusManager.ApplyEffect(target, new SlowEffect());
                    break;
                case TribeType.Nature:
                    statusManager.ApplyEffect(target, new PoisonEffect());
                    break;
                case TribeType.Air:
                    statusManager.ApplyEffect(target, new KnockbackEffect());
                    break;
            }
        }
    }
}