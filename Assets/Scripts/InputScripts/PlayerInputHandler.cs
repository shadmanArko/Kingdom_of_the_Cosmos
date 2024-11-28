using DBMS.RunningData;
using Player;
using Signals.BattleSceneSignals;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using zzz_TestScripts.Signals.BattleSceneSignals;

namespace InputScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputMaster _inputControls;

        [Inject] private readonly SignalBus _signalBus;

        private RunningDataScriptable _runningDataScriptable;

        private PlayerController _playerController;
        
        private Camera _camera;
        
        // private InputActionMap keyboardMap;
        // private InputActionMap gamepadMap;

        [SerializeField] private PlayerController unit;
        
        private Vector2 _moveInput;

        private InputAction _mousePositionAction;

        private bool _canTakeAttackInput;

        #region Initializers

        private void Awake()
        {
            _inputControls = new InputMaster();
            // keyboardMap = inp.actions.FindActionMap("Keyboard");
            // gamepadMap = playerInput.actions.FindActionMap("Gamepad");
        
            SubscribeToActions();
            
            _canTakeAttackInput = true;
        }

        [Inject]
        private void InitializeDiReference(Camera cam, RunningDataScriptable runningDataScriptable, PlayerController playerController)
        {
            _camera = cam;
            _runningDataScriptable = runningDataScriptable;
            _playerController = playerController;
        }

        private void SubscribeToActions()
        {
            _inputControls.PlayerControl.MeleeAttack.performed += _ => AttackInput();
            _inputControls.PlayerControl.Reload.performed += _ => Reload();
            _inputControls.PlayerControl.SwitchWeapon.performed += _ => SwitchWeapon();
            _inputControls.PlayerControl.Dash.performed += _ => StartDash();
            _inputControls.PlayerControl.Dash.canceled += _ => StopDash();
            _inputControls.PlayerControl.WeaponThrow.performed += _ => StartThrowWeapon();
            _inputControls.PlayerControl.WeaponThrow.canceled += _ => StopThrowWeapon();
            _inputControls.PlayerControl.ToggleAutoAttack.performed += _ => ToggleAutoAttack();
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
            
            _signalBus.Unsubscribe<MouseMovementSignal>(UpdateMouseMovement);
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
            if(!_canTakeAttackInput) return;
            _playerController.Attack(_runningDataScriptable.attackDirection);
        }
        
        private void Reload()
        {
            _signalBus.Fire<ReloadSignal>();
        }

        private void SwitchWeapon()
        {
            _signalBus.Fire<SwitchControlledWeaponSignal>();
        }

        #region Dash

        private void StartDash()
        {
            _signalBus.Fire<StartDashSignal>();
            Debug.Log("Dash started");
        }
        
        private void StopDash()
        {
            _signalBus.Fire<StopDashSignal>();
            Debug.Log("Dash ended");
        }

        #endregion

        #region Update Mouse Position
        
        private bool _mouseMoved;
        private Vector2 _lastMousePosition;
        private void ReadMousePosition()
        {
            if(!_canTakeAttackInput) return;
            var mousePos = _mousePositionAction.ReadValue<Vector2>();
            var mouseWorldPosition =
                _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _camera.nearClipPlane));
            var direction = (_playerController.gameObject.transform.position - mouseWorldPosition).normalized * -1;
            _signalBus.Fire(new MouseMovementSignal(direction));
        }
        
        private void UpdateMouseMovement(MouseMovementSignal mouseMovementSignal)
        {
            _runningDataScriptable.attackDirection = mouseMovementSignal.MousePos;
        }

        #endregion

        #region Weapon Throw

        private void StartThrowWeapon()
        {
            Debug.Log("Firing weapon throw START signal");
            _signalBus.Fire<WeaponThrowStartSignal>();
        }
        private void StopThrowWeapon()
        {
            _signalBus.Fire<WeaponThrowStopSignal>();
            Debug.Log("Firing weapon throw STOP signal");
        }

        #endregion

        #region Auto Attack

        private void ToggleAutoAttack()
        {
            _canTakeAttackInput = !_canTakeAttackInput;
            _runningDataScriptable.isAutoAttacking = !_canTakeAttackInput;
            _signalBus.Fire<ToggleAutoAttackSignal>();
            Debug.Log("Auto Attack Input Handler");
        }

        #endregion
    }
}