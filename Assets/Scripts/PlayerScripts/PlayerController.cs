using Cinemachine;
using InputScripts;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        
        public float moveSpeed = 5f;
        
        [SerializeField] private Vector2 movement;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerAnimationController playerAnimationController;
        [SerializeField] private PlayerInputHandler playerInputHandler;

        [Inject]
        private void InitializeDiReference(CinemachineVirtualCamera cineMachineVirtualCamera)
        {
            _cineMachineVirtualCamera = cineMachineVirtualCamera;
            _cineMachineVirtualCamera.Follow = gameObject.transform;
        }

        public void Move(Vector2 direction)
        {
            movement = direction.normalized;
            rb.velocity = movement * moveSpeed;
        }

        public void Attack(Vector2 direction)
        {
            
        }
    }
}
