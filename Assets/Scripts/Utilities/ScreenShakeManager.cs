using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace Utilities
{
    public class ScreenShakeManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cineMachineVirtualCamera;
        [SerializeField] private CinemachineBasicMultiChannelPerlin cineMachinePerlin;

        [SerializeField] private float shakeIntensity;
        [SerializeField] private float shakeTime;

        [SerializeField] private float timer;

        private void Start()
        {
            cineMachinePerlin = cineMachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cineMachinePerlin.m_AmplitudeGain = 0f;
        }

        public async void ShakeScreen()
        {
            timer = shakeTime;

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                StartCameraShake();
                await Task.Delay(1);
            }
            
            StopCameraShake();
        }

        private void StartCameraShake()
        {
            cineMachinePerlin.m_AmplitudeGain = shakeIntensity;
        }

        private void StopCameraShake()
        {
            cineMachinePerlin.m_AmplitudeGain = 0f;
            timer = 0f;
        }
    }
}
