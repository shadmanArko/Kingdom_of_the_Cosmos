using ObjectPool;
using PlayerScripts;
using Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace InputScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputMaster _inputControls;

        [SerializeField] private PlayerController unit;
        
        private Vector2 _moveInput;

        #region Initializers

        private void Awake()
        {
            _inputControls = new InputMaster();
            SubscribeToActions();
        }

        private void SubscribeToActions()
        {
            _inputControls.PlayerControl.MeleeAttack.performed += _ => MeleeAttackInput();
        }

        #region Enable and Disable

        private void OnEnable()
        {
            _inputControls.Enable();
        }

        private void OnDisable()
        {
            _inputControls.Disable();
        }

        #endregion

        #endregion

        #region Update

        private void Update()
        {
            _moveInput = _inputControls.PlayerControl.Movement.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            unit.Move(_moveInput);
        }

        #endregion
        
        private void MeleeAttackInput()
        {
            Debug.Log($"Shoot triggered");
            var bulletPoolManager = GameReferenceStorage.Instance.bulletPoolingManager;
            var screenShakeManager = GameReferenceStorage.Instance.screenShakeManager;
            var unitPos = (Vector2) unit.transform.position;
            var bullet = bulletPoolManager.Pool.Get();
            
            bullet.transform.position = unitPos;
            var mousePos = ReadMousePosition();
            Vector2 direction = mousePos - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            bullet.Initialize(bulletPoolManager.Pool, direction);
            screenShakeManager.ShakeScreen();
        }


        #region Utilities

        private static Vector3 ReadMousePosition()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var worldPosition = Camera.main!.ScreenToWorldPoint(mousePosition);
            return worldPosition;
        }

        #endregion
    }
}