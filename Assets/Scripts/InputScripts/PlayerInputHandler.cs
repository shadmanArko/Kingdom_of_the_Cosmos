using DBMS.RunningData;
using ObjectPool;
using ObjectPoolScripts;
using Player;
using Signals.BattleSceneSignals;
using TestScripts.WeaponsTest;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;

namespace InputScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputMaster _inputControls;

        [Inject] private readonly SignalBus _signalBus;

        private RunningDataScriptable _runningDataScriptable;
        private ScreenShakeManager _screenShakeManager;
        private AbilityPoolManager _abilityPoolManager;

        private PlayerController _playerController;
        
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
        private void InitializeDiReference(Camera cam, ScreenShakeManager screenShakeManager, BulletPoolingManager bulletPoolingManager, AbilityPoolManager abilityPoolManager, RunningDataScriptable runningDataScriptable, PlayerController playerController)
        {
            _screenShakeManager = screenShakeManager;
            _camera = cam;
            _abilityPoolManager = abilityPoolManager;
            _runningDataScriptable = runningDataScriptable;
            _playerController = playerController;
        }

        private void SubscribeToActions()
        {
            _inputControls.PlayerControl.MeleeAttack.performed += _ => AttackInput();
            _inputControls.PlayerControl.Reload.performed += _ => Reload();
            _inputControls.PlayerControl.SwitchWeapon.performed += _ => SwitchWeapon();
        }

        #region Enable and Disable

        private void OnEnable()
        {
            _inputControls.Enable();

            _mousePositionAction = _inputControls.PlayerControl.MousePosition;
            _mousePositionAction.Enable();
            
            _signalBus.Subscribe<MouseMovementSignal>(UpdateMouseMovement);
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
            _signalBus.Fire<MeleeAttackSignal>();
            
            _playerController.Attack(Vector2.zero);
            _screenShakeManager.ShakeScreen();
        }

        private void Reload()
        {
            _signalBus.Fire<ReloadSignal>();
        }

        private void SwitchWeapon()
        {
            _signalBus.Fire<SwitchControlledWeaponSignal>();
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
            _lastMousePosition = mousePos;
            var mouseWorldPosition =
                _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _camera.nearClipPlane));
            var direction = (_playerController.gameObject.transform.position - mouseWorldPosition).normalized;
            _signalBus.Fire(new MouseMovementSignal(direction));
            // Debug.Log($"Mouse position move signal fired {mousePos}");
        }

        #endregion

        private void UpdateMouseMovement(MouseMovementSignal mouseMovementSignal)
        {
            _runningDataScriptable.attackDirection = mouseMovementSignal.MousePos;
            // Debug.Log("Mouse Pos updated to running data scriptable");
        }
        
    }
}