using System;

namespace Player.Signals.ShieldSignals
{
    [Serializable]
    public class PlayerShieldIncreaseSignal
    {
        public float increaseValue;

        public PlayerShieldIncreaseSignal(float value)
        {
            increaseValue = value;
        }
    }
}