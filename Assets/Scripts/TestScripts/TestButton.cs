using UnityEngine;
using Utilities;
using Zenject;

public class TestButton : MonoBehaviour
{
    private ScreenShakeManager _screenShakeManager;

    [Inject]
    private void InstallDiReference(ScreenShakeManager screenShakeManager)
    {
        _screenShakeManager = screenShakeManager;
    }
    
    [ContextMenu("DoSomething")]
    private void DoSomething()
    {
        _screenShakeManager.ShakeScreen();
    }
}
