using UniRx;

namespace Experience
{
    public class ExpModel
    {
        private IReactiveProperty<float> _exp;
        public IReadOnlyReactiveProperty<float>Exp => _exp;
        private IReactiveProperty<float> _level;
        public IReadOnlyReactiveProperty<float>Level => _level;

        public void AddExp(double exp)
        {
            
        }

        private void UpgradeLevel()
        {
            
        }
    }
}