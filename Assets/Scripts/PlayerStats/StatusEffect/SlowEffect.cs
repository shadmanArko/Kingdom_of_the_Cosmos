using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public class SlowEffect : BaseStatusEffect
    {
        private readonly float _slowPercentage;
    private float _originalSpeed;

    public SlowEffect(float duration = 3f, float tickInterval = 0.5f, float slowPercentage = 30f) 
        : base(duration, tickInterval)
    {
        _slowPercentage = slowPercentage;
    }

    public override void Apply(GameObject target)
    {
        base.Apply(target);
        
        if (target.TryGetComponent<IMoveable>(out var moveable))
        {
            _originalSpeed = moveable.MovementSpeed;
            moveable.MovementSpeed *= (1 - _slowPercentage / 100f);
        }
    }

    public override void Tick(GameObject target) 
    {
        // Slow effect is persistent, no tick needed
    }

    public override void Remove(GameObject target)
    {
        base.Remove(target);
        if (target.TryGetComponent<IMoveable>(out var moveable))
        {
            moveable.MovementSpeed = _originalSpeed;
        }
    }
}
}