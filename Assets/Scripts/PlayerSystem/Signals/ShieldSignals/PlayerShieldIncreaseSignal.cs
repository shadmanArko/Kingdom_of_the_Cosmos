using System;

namespace PlayerSystem.Signals.ShieldSignals
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