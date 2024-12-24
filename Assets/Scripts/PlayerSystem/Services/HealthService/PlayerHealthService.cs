using PlayerSystem.PlayerSO;
using UnityEngine;

namespace PlayerSystem.Services.HealthService
{
    public class PlayerHealthService
    {
        #region Health

        public void IncreasePlayerHealth()
        {
            
        }

        public void ReducePlayerHealth()
        {
            
        }

        #endregion

        #region Shield

        private void IncreaseShield()
        {
            
        }

        private void ReduceShield()
        {
            
        }

        #endregion

        public void TakeDamage(PlayerScriptableObject playerScriptableObject, float damageAmount)
        {
            var canTakeDamage = CheckTakeDamageEligibility();
        }

        private bool CheckTakeDamageEligibility()
        {
            return true;
        }
    }
}