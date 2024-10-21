using Cinemachine;
using InputScripts;
using UnityEngine;
using Zenject;

namespace Player
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
            
            if(rb.velocity.magnitude > 0)
                playerAnimationController.PlayAnimation("run");
            else
            {
                var stateInfo = playerAnimationController.animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.length == 0 || stateInfo.normalizedTime >= 1.0f)
                {
                    playerAnimationController.PlayAnimation("idle");
                }
            }
        }

        public void Attack(Vector2 direction)
        {
            playerAnimationController.PlayAnimation("attack");
        }
    }
}
