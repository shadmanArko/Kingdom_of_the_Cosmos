using ObjectPool;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Zenject;

namespace InputScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputMaster _inputControls;

        private BulletPoolingManager _bulletPoolingManager;
        private ScreenShakeManager _screenShakeManager;
        
        private InputActionMap keyboardMap;
        private InputActionMap gamepadMap;

        [SerializeField] private PlayerController unit;
        
        private Vector2 _moveInput;

        #region Initializers

        private void Awake()
        {
            _inputControls = new InputMaster();
            // keyboardMap = inp.actions.FindActionMap("Keyboard");
            // gamepadMap = playerInput.actions.FindActionMap("Gamepad");
        
            SubscribeToActions();
        }

        [Inject]
        private void InitializeDiReference(ScreenShakeManager screenShakeManager, BulletPoolingManager bulletPoolingManager)
        {
            _screenShakeManager = screenShakeManager;
            _bulletPoolingManager = bulletPoolingManager;
        }

        private void SubscribeToActions()
        {
            _inputControls.PlayerControl.MeleeAttack.performed += _ => AttackInput();
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
        
        private void AttackInput()
        {
            Debug.Log($"Shoot triggered");
            
            var unitPos = (Vector2) unit.transform.position;
            var bullet = _bulletPoolingManager.Pool.Get();
            
            bullet.transform.position = unitPos;
            var mousePos = ReadMousePosition();
            Vector2 direction = mousePos - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            bullet.Initialize(_bulletPoolingManager.Pool, direction);
            _screenShakeManager.ShakeScreen();
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