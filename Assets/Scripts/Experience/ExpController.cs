using PlayerSystem.Views;
using UniRx;

namespace Experience
{
    public class ExpController
    {
        private readonly ExpModel _model;

        public ExpController(ExpModel model, PlayerStatView view, CompositeDisposable disposable)
        {
            _model = model;

            model.Level.Subscribe(level => view.Level = level.ToString()).AddTo(disposable);
            
            model.CollectedExp.Subscribe(exp => view.CollectedExp = exp.ToString()).AddTo(disposable);
            model.CollectedExp.Subscribe(_ => view.ExpSliderValueInPercentage = model.ExpSliderValueInPercentage()).AddTo(disposable);
            
            model.MaxExp.Subscribe(exp => view.MaxExp = exp.ToString()).AddTo(disposable);
        }

        public void AddExp(float exp)
        {
            _model.AddExp(exp);
        }
    }
}