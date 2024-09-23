using ObjectPool;
using ObjectPoolScripts;
using UnityEngine;

namespace Utilities
{
    public class GameReferenceStorage : MonoBehaviour
    {
        public static GameReferenceStorage Instance;

        private void Awake()
        {
            if(Instance != null) return;
            Instance = this;
        }

        public ScreenShakeManager screenShakeManager;
        
        public BulletPoolingManager bulletPoolingManager;
    }
}