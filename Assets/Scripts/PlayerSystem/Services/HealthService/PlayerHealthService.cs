using PlayerSystem.Models;
using PlayerSystem.PlayerSO;
using UnityEngine;

namespace PlayerSystem.Services.HealthService
{
    public class PlayerHealthService
    {
        #region Health

        public void SetHealth(Player player, float health)
        {
            player.health = Mathf.Clamp(health, 0, player.maxHealth);
        }

        public void IncreaseHealth(Player player, float value)
        {
            var health = player.health + value;
            health = Mathf.Clamp(health, 0, player.maxHealth);
            player.health = health;
        }

        private void ReduceHealth(Player player, float damageAmount)
        {
            var health = player.health - damageAmount;
            health = Mathf.Clamp(health, 0, player.maxHealth);
            player.health = health;
        }

        #endregion

        #region Shield

        public void SetShield(Player player, float value)
        {
            player.shield = Mathf.Clamp(value, 0, player.maxHealth);
        }

        public void IncreaseShield(Player player, float value)
        {
            var shield = player.shield + value;
            shield = Mathf.Clamp(shield, 0, player.maxShield);
            player.shield = shield;
        }

        private void ReduceShield(Player player, float damageAmount)
        {
            var shield = player.shield - damageAmount;
            shield = Mathf.Clamp(shield, 0, player.maxShield);
            player.shield = shield;
        }

        #endregion

        public void TakeDamage(PlayerScriptableObject playerScriptableObject, float damageAmount)
        {
            var canTakeDamage = CheckTakeDamageEligibility();
            if(!canTakeDamage) return;
            var player = playerScriptableObject.player;
            
            if (player.shield <= 0)
                ReduceHealth(player, damageAmount);
            else if(player.shield < damageAmount)
            {
                var healthReduceAmount = damageAmount - player.shield;
                ReduceShield(player, damageAmount);
                ReduceHealth(player, healthReduceAmount);
            }
            else
                ReduceShield(player, damageAmount);
        }

        private bool CheckTakeDamageEligibility()
        {
            return true;
        }
    }
}