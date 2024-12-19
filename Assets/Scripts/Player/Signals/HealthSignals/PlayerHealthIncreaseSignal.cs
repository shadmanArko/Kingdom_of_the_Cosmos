using System;

namespace Player.Signals.HealthSignals
{
    [Serializable]
    public class PlayerHealthIncreaseSignal
    {
        public float increaseValue;
        public PlayerHealthIncreaseSignal(float value)
        {
            increaseValue = value;
        }
    }
}