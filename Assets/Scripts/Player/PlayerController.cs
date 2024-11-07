using System.Threading.Tasks;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private CinemachineVirtualCamera _cineMachineVirtualCamera;
        private SignalBus _signalBus;
        
        public float speed = 5f;

        #region Dash Variables

        [SerializeField] private int dashCount;
        [SerializeField] private int totalDashCount;
        
        [SerializeField] private bool isDashing;
        [SerializeField] private bool canDash;
        
        [SerializeField] private float dashSpeed = 25f;
        [SerializeField] private float dashDuration;
        
        [SerializeField] private float dashCooldownDuration;

        #endregion

        #region Move Variables

        public float moveSpeed = 5f;
        [SerializeField] private Vector2 movement;
        [SerializeField] private bool canMove;

        #endregion
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerAnimationController playerAnimationController;

        [Inject]
        private void InitializeDiReference(CinemachineVirtualCamera cineMachineVirtualCamera, SignalBus signalBus)
        {
            _cineMachineVirtualCamera = cineMachineVirtualCamera;
            _cineMachineVirtualCamera.Follow = gameObject.transform;
            _signalBus = signalBus;
        }
        
        private void Start()
        {
            SubscribeToActions();
            canMove = true;
            isDashing = false;
            canDash = true;
            speed = moveSpeed;
            
            dashDuration = 0.8f;

            dashCooldownDuration = 2f;
            totalDashCount = 2;
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _signalBus.Subscribe<StartDashSignal>(StartDash);
            _signalBus.Subscribe<StopDashSignal>(StopDash);
        }
        
        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<StartDashSignal>(StartDash);
            _signalBus.Unsubscribe<StopDashSignal>(StopDash);
        }

        #endregion
        
        public void Move(Vector2 direction)
        {
            if(!canMove) return;
            
            if (!isDashing)
            {
                movement = direction.normalized;
                rb.velocity = movement * speed;
            }
            else
            {
                Debug.Log("Dashing...");
                movement = direction.normalized;
                movement = movement == Vector2.zero ? Vector2.right : movement;
                rb.velocity = movement * speed;
            }
            
            if(rb.velocity.magnitude > 0)
                playerAnimationController.PlayAnimation( isDashing ? "dash" : "run");
            else
            {
                var stateInfo = playerAnimationController.animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.length == 0 || stateInfo.normalizedTime >= 1.0f)
                {
                    playerAnimationController.PlayAnimation("idle");
                }
            }
        }

        #region Attack

        [SerializeField] private bool canAttack;
        public void Attack(Vector2 direction)
        {
            if(!canAttack) return;
            playerAnimationController.PlayAnimation("attack");
        }

        #endregion

        #region Dash
        
        private void StartDash()
        {
            if(!canDash) return;
            if(dashCount <= 0) return;
            Debug.Log("Start Dash called");
            speed = dashSpeed;
            canAttack = false;
            isDashing = true;
            StartDashCountdown();
        }

        private void StopDash()
        {
            if(!isDashing) return;
            Debug.Log("Stop Dash called");
            speed = moveSpeed;
            canAttack = true;
            isDashing = false;
        }

        private async void StartDashCountdown()
        {
            await Task.Delay(Mathf.CeilToInt(dashDuration * 1000));
            StopDash();
            StartDashCooldown();
        }

        private async void StartDashCooldown()
        {
            dashCount -= 1;
            await Task.Delay(Mathf.CeilToInt(dashCooldownDuration * 1000));
            dashCount += 1;
            dashCount = math.clamp(dashCount, 0, totalDashCount);
        }

        #endregion

        private void OnDestroy()
        {
            UnsubscribeToActions();
        }
    }
}
