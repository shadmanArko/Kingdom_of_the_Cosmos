using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public class PoisonEffect : BaseStatusEffect
    {
        private readonly float _damagePerTick;
        private readonly float _stackMultiplier;
        private int _currentStacks;
        private const int MaxStacks = 3;

        public PoisonEffect(float duration = 4f, float tickInterval = 0.5f, float damagePerTick = 5f, float stackMultiplier = 1.5f) 
            : base(duration, tickInterval)
        {
            _damagePerTick = damagePerTick;
            _stackMultiplier = stackMultiplier;
        }

        public override void Apply(GameObject target)
        {
            if (IsActive)
            {
                // If already poisoned, add stack instead of reapplying
                _currentStacks = Mathf.Min(_currentStacks + 1, MaxStacks);
                RemainingDuration = Duration;
                return;
            }

            base.Apply(target);
            _currentStacks = 1;
        }

        public override void Tick(GameObject target)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                float stackedDamage = _damagePerTick * Mathf.Pow(_stackMultiplier, _currentStacks - 1);
                damageable.TakeDamage(new DamageInfo
                {
                    Amount = stackedDamage,
                    Type = DamageType.Poison,
                    IsDot = true
                });
            }
        }
    }
}