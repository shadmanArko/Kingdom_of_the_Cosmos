using System;

namespace Player.Signals
{
    [Serializable]
    public class PlayerTakeDamageSignal
    {
        public float damageValue;

        public PlayerTakeDamageSignal(float value)
        {
            damageValue = value;
        }
    }
}