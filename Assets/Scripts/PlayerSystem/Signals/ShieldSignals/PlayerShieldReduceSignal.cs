using System;

namespace PlayerSystem.Signals.ShieldSignals
{
    [Serializable]
    public class PlayerShieldReduceSignal
    {
        public float reduceValue;

        public PlayerShieldReduceSignal(float value)
        {
            reduceValue = value;
        }
    }
}