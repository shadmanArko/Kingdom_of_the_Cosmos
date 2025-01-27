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

        public ExpModel()
        {
            //TODO: To be changed later
            _level = new ReactiveProperty<float>(0);
            _collectedExp = new ReactiveProperty<float>(0);
            _maxExp = new ReactiveProperty<float>(100);
        }

        public void AddExp(float exp)
        {
            _collectedExp.Value += exp;
            if (!(_collectedExp.Value >= _maxExp.Value)) return;
            var extraExp = _collectedExp.Value % _maxExp.Value;
            _level.Value++;
            _collectedExp.Value = extraExp;
            _maxExp.Value += 20;
        }

        public float ExpSliderValueInPercentage() => _collectedExp.Value / _maxExp.Value * 100f;
    }
}