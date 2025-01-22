using System;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

namespace PlayerStats
{
    public class PlayerStatsController : IPlayerStatsController
    {
        private readonly PlayerStatsModel _model;
        private readonly PlayerStatsView _view;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public PlayerStatsController(PlayerStatsModel model, PlayerStatsView view)
        {
            _model = model;
            _view = view;
        
            // Subscribe to stat changes
            foreach (StatType statType in Enum.GetValues(typeof(StatType)))
            {
                _model.GetStatValue(statType)
                    .Subscribe(value => _view.UpdatePlayerStat(statType, value))
                    .AddTo(_disposables);
            }
        }

        public void HandleStatPickup(StatPickup pickup)
        {
            if (pickup.IsPermanent)
            {
                ModifyPermanentStat(pickup.StatType, pickup.Value);
            }
            else
            {
                ApplyTemporaryStat(pickup.StatType, pickup.Value, pickup.Duration).Forget();
            }
        }

        public async UniTask ApplyTemporaryStat(StatType statType, float value, float duration)
        {
            await _model.ApplyTemporaryStat(statType, value, duration);
        }

        public void ModifyPermanentStat(StatType statType, float value)
        {
            _model.ModifyPermanentStat(statType, value);
        }
        
        public void SetTribe(TribeType tribe)
        {
            _model.SetTribe(tribe);
        }

        public void SetWeapon(WeaponType weapon)
        {
            _model.SetWeapon(weapon);
        }
    }
}