using ObjectPool;
using ObjectPoolScripts;
using PlayerScripts;
using Signals.BattleSceneSignals;
using TestScripts.WeaponsTest;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Zenject;

namespace InputScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputMaster _inputControls;

        [Inject] private readonly SignalBus _signalBus;
        private ScreenShakeManager _screenShakeManager;
        private AbilityPoolManager _abilityPoolManager;
        private Camera _camera;
        
        private InputActionMap keyboardMap;
        private InputActionMap gamepadMap;

        [SerializeField] private PlayerController unit;
        
        private Vector2 _moveInput;

        private InputAction _mousePositionAction;

        #region Initializers

        private void Awake()
        {
            _inputControls = new InputMaster();
            // keyboardMap = inp.actions.FindActionMap("Keyboard");
            // gamepadMap = playerInput.actions.FindActionMap("Gamepad");
        
            SubscribeToActions();
        }

        [Inject]
        private void InitializeDiReference(Camera cam, ScreenShakeManager screenShakeManager, BulletPoolingManager bulletPoolingManager, AbilityPoolManager abilityPoolManager)
        {
            _screenShakeManager = screenShakeManager;
            _camera = cam;
            _abilityPoolManager = abilityPoolManager;
        }

        private void SubscribeToActions()
        {
            _inputControls.PlayerControl.MeleeAttack.performed += _ => AttackInput();
        }

        #region Enable and Disable

        private void OnEnable()
        {
            _inputControls.Enable();

            _mousePositionAction = _inputControls.PlayerControl.MousePosition;
            _mousePositionAction.Enable();
        }

        private void OnDisable()
        {
            _inputControls.Disable();
            _mousePositionAction.Disable();
            _lastMousePosition = _mousePositionAction.ReadValue<Vector2>();
        }

        #endregion

        #endregion

        #region Update

        private void Update()
        {
            _moveInput = _inputControls.PlayerControl.Movement.ReadValue<Vector2>();
            ReadMousePosition();
        }

        private void FixedUpdate()
        {
            unit.Move(_moveInput);
        }

        #endregion
        
        private void AttackInput()
        {
            Debug.Log($"Shoot triggered");
            
            // _signalBus.Fire<MeleeAttackSignal>();
            
            _screenShakeManager.ShakeScreen();
            
            _abilityPoolManager.ActivateAbility(ReadPreviousMousePosition());
            // lastActivationTime = Time.time;
        }


        #region Utilities

        private Vector3 ReadPreviousMousePosition()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var worldPosition = _camera!.ScreenToWorldPoint(mousePosition);
            return worldPosition;
        }

        private Vector3 _lastMousePosition;
        private bool _mouseMoved;
        private readonly float _movementThreshold = 0.01f;
        private void ReadMousePosition()
        {
            var mousePos = _mousePositionAction.ReadValue<Vector2>();
            var distance = Vector3.Distance(mousePos, _lastMousePosition);

            if (!(distance > _movementThreshold)) return;
            _lastMousePosition = mousePos;  // Update position
            Vector3 mouseWorldPosition =
                _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _camera.nearClipPlane));
            _signalBus.Fire(new MouseMovementSignal(mouseWorldPosition));
            Debug.Log($"Mouse position move signal fired {mousePos}");
        }

        #endregion
        
    }
}