using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public class BurnEffect : BaseStatusEffect
    {
        private readonly float _damagePerTick;
        private ParticleSystem _burnParticles;

        public BurnEffect(float duration = 5f, float tickInterval = 1f, float damagePerTick = 10f) 
            : base(duration, tickInterval)
        {
            _damagePerTick = damagePerTick;
        }

        public override void Apply(GameObject target)
        {
            base.Apply(target);
        
            // Spawn burn particles
            if (target.TryGetComponent<ParticleSystem>(out var particleSystem))
            {
                _burnParticles = particleSystem;
                _burnParticles.Play();
            }
        }

        public override void Tick(GameObject target)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(new DamageInfo
                {
                    Amount = _damagePerTick,
                    Type = DamageType.Fire,
                    IsDot = true
                });
            }
        }

        public override void Remove(GameObject target)
        {
            base.Remove(target);
            if (_burnParticles != null)
            {
                _burnParticles.Stop();
            }
        }
    }
}