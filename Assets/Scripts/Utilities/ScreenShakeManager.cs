using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace Utilities
{
    public class ScreenShakeManager
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin _cineMachinePerlin;

        private float _shakeIntensity;
        private float _shakeTime;

        private float _timer;

        public ScreenShakeManager(CinemachineVirtualCamera cineMachineVirtualCamera)
        {
            _cineMachineVirtualCamera = cineMachineVirtualCamera;
            
            _cineMachinePerlin = _cineMachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _cineMachinePerlin.m_AmplitudeGain = 0f;

            _shakeIntensity = 0.2f;
            _shakeTime = 0.5f;
            _timer = _shakeTime;
        }

        public async void ShakeScreen()
        {
            _timer = _shakeTime;

            Debug.Log("Screen Shaking!!");
            while (_timer > 0)
            {
                _timer -= Time.deltaTime;
                StartCameraShake();
                await Task.Delay(1);
            }
            
            StopCameraShake();
        }

        private void StartCameraShake()
        {
            _cineMachinePerlin.m_AmplitudeGain = _shakeIntensity;
        }

        private void StopCameraShake()
        {
            _cineMachinePerlin.m_AmplitudeGain = 0f;
            _timer = 0f;
        }
    }
}
