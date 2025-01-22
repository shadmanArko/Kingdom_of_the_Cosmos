using PlayerSystem.Models;
using UnityEngine;
using Zenject;

namespace PlayerStats
{
    public class PlayerStatsView : MonoBehaviour
    {
        private Player _player;
        private IPlayerStatsController _controller;
    
        [Inject]
        public void Construct(Player player, IPlayerStatsController controller)
        {
            _player = player;
            _controller = controller;
        }

        public void OnStatPickup(StatPickup pickup)
        {
            _controller.HandleStatPickup(pickup);
        }

        // Update player stats based on model changes
        public void UpdatePlayerStat(StatType statType, float value)
        {
            switch (statType)
            {
                case StatType.Health:
                    _player.health = value;
                    break;
                case StatType.MaxHealth:
                    _player.maxHealth = value;
                    break;
                case StatType.Shield:
                    _player.shield = value;
                    break;
                // Add other stat updates here
            }
        }
    }
}