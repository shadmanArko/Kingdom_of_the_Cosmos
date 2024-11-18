using System.Threading.Tasks;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;
using Vector2 = UnityEngine.Vector2;

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
        
        [SerializeField] private float lungeDashSpeed = 20f;
        [SerializeField] private float rollDashSpeed = 10f;
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
        }
        
        private void UnsubscribeToActions()
        {
            _signalBus.Unsubscribe<StartDashSignal>(StartDash);
        }

        #endregion
        
        public void Move(Vector2 direction)
        {
            if(!canMove) return;
            movement = direction.normalized;
            
            if (!isDashing)
            {
                movement = direction.normalized;
                rb.velocity = movement * speed;
            }
            else
            {
                Debug.Log("Dashing...");
                // movement = direction.normalized;
                // movement = movement == Vector2.zero ? Vector2.right : movement;
                // _dashDirection = _dashDirection == Vector2.zero ? movement.normalized : _dashDirection;
                _dashDirection = isRollDashing ? direction.normalized : _dashDirection;
                rb.velocity = _dashDirection * speed;
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
            canAttack = false;
            isDashing = true;
            
            LungeDash();
        }

        private Vector2 _dashDirection;
        [SerializeField] private float lungeDashTime = 0.4f;
        [SerializeField] private float rollDashTime = 0.4f;
        
        [SerializeField] private bool isLungeDashing;
        [SerializeField] private bool isRollDashing;
        
        private async void LungeDash()
        {
            isDashing = true;
            _dashDirection = movement.normalized;
            isLungeDashing = true;
            speed = lungeDashSpeed;
            Debug.Log($"Lunge Dashing direction: {_dashDirection}");
            await Task.Delay(Mathf.CeilToInt(lungeDashTime * 1000));
            isLungeDashing = false;
            
            RollDash();
        }

        private async void RollDash()
        {
            _dashDirection = movement.normalized;
            isRollDashing = true;
            speed = rollDashSpeed;
            Debug.Log($"Roll Dashing direction: {_dashDirection}");
            await Task.Delay(Mathf.CeilToInt(rollDashTime * 1000));
            isRollDashing = false;
            
            StopDash();
            StartDashCooldown();
        }

        private void StopDash()
        {
            if(!isDashing) return;
            Debug.Log("Stop Dash called");
            speed = moveSpeed;
            canAttack = true;
            isDashing = false;
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
