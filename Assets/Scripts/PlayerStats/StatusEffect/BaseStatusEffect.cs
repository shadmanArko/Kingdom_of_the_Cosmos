using System;
using UniRx;
using UnityEngine;

namespace PlayerStats.StatusEffect
{
    public abstract class BaseStatusEffect: IStatusEffect
    {
        public bool IsActive { get; protected set; }
        public float Duration { get; protected set; }
        public float TickInterval { get; protected set; }
        protected float RemainingDuration;
        protected CompositeDisposable Disposables = new();

        protected BaseStatusEffect(float duration, float tickInterval)
        {
            Duration = duration;
            TickInterval = tickInterval;
        }

        public virtual void Apply(GameObject target)
        {
            IsActive = true;
            RemainingDuration = Duration;

            // Setup tick system
            Observable.Interval(TimeSpan.FromSeconds(TickInterval))
                .TakeWhile(_ => IsActive && RemainingDuration > 0)
                .Subscribe(_ =>
                {
                    Tick(target);
                    RemainingDuration -= TickInterval;
                
                    if (RemainingDuration <= 0)
                    {
                        Remove(target);
                    }
                })
                .AddTo(Disposables);
        }

        public virtual void Remove(GameObject target)
        {
            IsActive = false;
            Disposables.Clear();
        }

        public abstract void Tick(GameObject target);
    }
}