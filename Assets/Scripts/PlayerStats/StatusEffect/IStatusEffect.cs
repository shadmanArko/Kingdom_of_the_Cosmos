using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public interface IStatusEffect
    {
        bool IsActive { get; }
        float Duration { get; }
        float TickInterval { get; }
        void Apply(GameObject target);
        void Remove(GameObject target);
        void Tick(GameObject target);
    }
}