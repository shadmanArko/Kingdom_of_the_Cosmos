using System;
using DBMS.RunningData;
using Player.Signals.HealthSignals;
using Player.Signals.ShieldSignals;
using Player.Views;
using UnityEngine;
using Zenject;

namespace Player.Services.HealthService
{
    public class PlayerHealthService : IInitializable, IDisposable
    {
        private readonly RunningDataScriptable _runningDataScript;
        private readonly PlayerHealthView _playerHealthView;
        private readonly SignalBus _signalBus;

        public void Initialize()
        {
           SubscribeToActions();
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<PlayerHealthIncreaseSignal>(IncreasePlayerHealth);
            _signalBus.Subscribe<PlayerHealthReduceSignal>(ReducePlayerHealth);
            _signalBus.Subscribe<PlayerShieldIncreaseSignal>(IncreaseShield);
            _signalBus.Subscribe<PlayerShieldReduceSignal>(ReduceShield);
        }

        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<PlayerHealthIncreaseSignal>(IncreasePlayerHealth);
            _signalBus.Unsubscribe<PlayerHealthReduceSignal>(ReducePlayerHealth);
            _signalBus.Unsubscribe<PlayerShieldIncreaseSignal>(IncreaseShield);
            _signalBus.Unsubscribe<PlayerShieldReduceSignal>(ReduceShield);
        }

        #endregion

        #region Health

        private void IncreasePlayerHealth(PlayerHealthIncreaseSignal playerHealthIncreaseSignal)
        {
            _playerHealthView.HealthBar += playerHealthIncreaseSignal.increaseValue;
            Debug.Log("Player health increased");
        }

        private void ReducePlayerHealth(PlayerHealthReduceSignal playerHealthReduceSignal)
        {
            _playerHealthView.HealthBar -= playerHealthReduceSignal.reduceValue;
            Debug.Log("Player health reduced");
        }

        #endregion

        #region Shield

        private void IncreaseShield(PlayerShieldIncreaseSignal playerShieldIncreaseSignal)
        {
            _playerHealthView.ShieldBar += playerShieldIncreaseSignal.increaseValue;
            Debug.Log("Player shield increased");
        }

        private void ReduceShield(PlayerShieldReduceSignal playerShieldReduceSignal)
        {
            _playerHealthView.ShieldBar -= playerShieldReduceSignal.reduceValue;
            Debug.Log("Player shield reduced");
        }

        #endregion

        public void Dispose()
        {
            UnsubscribeToActions();
        }
    }
}