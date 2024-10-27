namespace zzz_TestScripts.Signals.UiSignals
{
    public class SceneChangeSignal
    {
        public string SceneName { get; }

        public SceneChangeSignal(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}