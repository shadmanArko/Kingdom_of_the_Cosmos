using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using zzz_TestScripts.Signals.UiSignals;

namespace zzz_TestScripts.UniRxEventDrivenUI
{
    public class MainMenuUiUniRx : MonoBehaviour
    {
        private SignalBus _signalBus;
        
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        // [SerializeField] private string gameSceneName;

        [Inject]
        private void InitializeDiReferences(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            _signalBus.Subscribe<SceneChangeSignal>(LoadGameScene);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            startButton.OnClickAsObservable().Subscribe(_ =>
            {
                // Debug.Log("Start Game button clicked, loading scene...");
                // LoadGameScene();
                _signalBus.Fire(new SceneChangeSignal("objectpool enemy test"));
                
            }).AddTo(this);

            optionsButton.OnClickAsObservable().Subscribe(_ => { Debug.Log("Options Button Pressed"); }).AddTo(this);

            quitButton.OnClickAsObservable().Subscribe(_ =>
            {
                Debug.Log("Quit Game Method Called");
                QuitGame();
            }).AddTo(this);
        }

        private void QuitGame()
        {
            Application.Quit();
        }

        private void LoadGameScene(SceneChangeSignal sceneChangeSignal)
        {
            Debug.Log("Start Game button clicked, loading scene...");
            SceneManager.LoadSceneAsync(sceneChangeSignal.SceneName);
        }
    }
}