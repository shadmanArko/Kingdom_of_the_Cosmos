using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public class KnockbackEffect : BaseStatusEffect
    {
        private readonly float _force;
        private readonly float _upwardForce;

        public KnockbackEffect(float duration = 0.5f, float force = 10f, float upwardForce = 5f) 
            : base(duration, 0)
        {
            _force = force;
            _upwardForce = upwardForce;
        }

        public override void Apply(GameObject target)
        {
            base.Apply(target);
        
            if (target.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = (target.transform.position - Camera.main.transform.position).normalized;
                direction.y = 0;
            
                rb.AddForce(direction * _force + Vector3.up * _upwardForce, ForceMode.Impulse);
            }
        }

        public override void Tick(GameObject target)
        {
            // Knockback is instant, no tick needed
        }
    }
}