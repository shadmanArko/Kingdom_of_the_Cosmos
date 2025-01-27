using PlayerSystem.PlayerSO;
using PlayerSystem.Services.HealthService;
using UniRx;

namespace Experience
{
    public class ExpModel
    {
        private IReactiveProperty<float> _level;
        public IReadOnlyReactiveProperty<float>Level => _level;

        private IReactiveProperty<float> _collectedExp;
        public IReadOnlyReactiveProperty<float> CollectedExp => _collectedExp;

        private IReactiveProperty<float> _maxExp;
        public IReadOnlyReactiveProperty<float> MaxExp => _maxExp;
        
        private readonly PlayerHealthService _playerHealthService;
        private readonly PlayerScriptableObject _playerScriptableObject;

        public ExpModel(PlayerHealthService playerHealthService, PlayerScriptableObject playerScriptableObject)
        {
            //TODO: To be changed later
            _level = new ReactiveProperty<float>(0);
            _collectedExp = new ReactiveProperty<float>(0);
            _maxExp = new ReactiveProperty<float>(100);
            _playerHealthService = playerHealthService;
            _playerScriptableObject = playerScriptableObject;
        }

        public void AddExp(float exp)
        {
            _collectedExp.Value += exp;
            if (!(_collectedExp.Value >= _maxExp.Value)) return;
            var extraExp = _collectedExp.Value % _maxExp.Value;
            _level.Value++;
            _collectedExp.Value = extraExp;
            _maxExp.Value += 20;
            _playerHealthService.SetShield(_playerScriptableObject.player, _playerScriptableObject.player.maxShield);
        }

        public float ExpSliderValueInPercentage() => _collectedExp.Value / _maxExp.Value * 100f;
    }
}