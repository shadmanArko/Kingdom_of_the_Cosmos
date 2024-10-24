using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace zzz_TestScripts.UniRxEventDrivenUI
{
    public class MainMenuUiUniRx : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private string gameSceneName;

        private void Start()
        {
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            startButton.OnClickAsObservable().Subscribe(_ =>
            {
                Debug.Log("Start Game button clicked, loading scene...");
                LoadGameScene();
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

        private void LoadGameScene()
        {
            SceneManager.LoadSceneAsync(gameSceneName);
        }
    }
}