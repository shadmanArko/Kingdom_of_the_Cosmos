using System;

namespace Player.Signals.ShieldSignals
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