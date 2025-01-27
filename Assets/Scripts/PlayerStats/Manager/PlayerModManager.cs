using PlayerSystem.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerStats.Manager
{
    public class PlayerModManager : ITickable
    {
        private readonly PlayerMod _playerMod;

        public PlayerModManager(PlayerMod playerMod)
        {
            _playerMod = playerMod;
            ApplySpeedBoost();
            SubscribeToHealthChanges();
        }

        public void Tick()
        {
            _playerMod.UpdateTemporaryModifications();
            Debug.LogWarning($"Movement Speed of player is: {_playerMod.BasePlayer.player.movementSpeed}");
        }

        public void ApplySpeedBoost()
        {
            // Permanent modification
            _playerMod.ModifyPermanentStat(nameof(Player.movementSpeed), 5f);

            // Temporary modification
            _playerMod.ApplyTemporaryStat(nameof(Player.movementSpeed), 10f, 5f);
        }

        public void SubscribeToHealthChanges()
        {
            _playerMod.GetStatObservable(nameof(Player.health))
                .Subscribe(newHealth => {
                    Debug.Log($"Health changed to: {newHealth}");
                });
        }
    }
}