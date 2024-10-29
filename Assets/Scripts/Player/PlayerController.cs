using System;
using Cinemachine;
using InputScripts;
using UnityEngine;
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

        [SerializeField] private bool isDashing;
        [SerializeField] private bool canDash;
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashTimeRemaining;
        [SerializeField] private float dashSpeed = 25f;
        [SerializeField] private float dashCooldown;
        [SerializeField] private float dashCooldownDuration;

        #endregion

        #region Move Variables

        public float moveSpeed = 5f;
        [SerializeField] private Vector2 movement;
        [SerializeField] private bool canMove;

        #endregion
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerAnimationController playerAnimationController;
        // [SerializeField] private PlayerInputHandler playerInputHandler;

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
            dashTimeRemaining = 0f;

            dashCooldownDuration = 2f;
            dashCooldown = dashCooldownDuration;
            dashTimeRemaining = dashDuration;
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

        private void FixedUpdate()
        {
            CheckDashValidity();
        }

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

        private bool _canAttack;
        public void Attack(Vector2 direction)
        {
            if(!_canAttack) return;
            playerAnimationController.PlayAnimation("attack");
        }

        #endregion

        #region Dash
        
        private void CheckDashValidity()
        {
            if (isDashing)
            {
                if (dashTimeRemaining <= 0)
                {
                    StopDash();
                }
                else
                {
                    dashTimeRemaining -= Time.fixedDeltaTime;
                }
            }
            
            dashCooldown -= Time.fixedDeltaTime;
            dashCooldown = Mathf.Clamp(dashCooldown, 0f, dashCooldownDuration);
            canDash = dashCooldown <= 0;
        }

        private void StartDash()
        {
            if(!canDash) return;
            Debug.Log("Start Dash called");
            speed = dashSpeed;
            _canAttack = false;
            canDash = false;
            isDashing = true;
            dashTimeRemaining = dashDuration;
            dashCooldown = 0f;
        }

        private void StopDash()
        {
            Debug.Log("Stop Dash called");
            speed = moveSpeed;
            _canAttack = true;
            isDashing = false;
            canDash = false;
            dashTimeRemaining = 0;
            dashCooldown = dashCooldownDuration;
        }

        #endregion

        private void OnDestroy()
        {
            UnsubscribeToActions();
        }
    }
}
