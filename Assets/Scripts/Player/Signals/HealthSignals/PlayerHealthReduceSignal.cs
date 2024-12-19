using System;

namespace Player.Signals.HealthSignals
{
    [Serializable]
    public class PlayerHealthReduceSignal
    {
        public float reduceValue;

        public PlayerHealthReduceSignal(float value)
        {
            reduceValue = value;
        }
    }
}