using DBMS.RunningData;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Signals.InputSignals;
using PlayerSystem.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace InputScripts
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputMaster _inputControls;

        [Inject] private readonly SignalBus _signalBus;

        private RunningDataScriptable _runningDataScriptable;
        
        private PlayerView _playerView;
        
        private Camera _camera;
        
        // private InputActionMap keyboardMap;
        // private InputActionMap gamepadMap;
        
        private Vector2 _moveInput;

        private InputAction _mousePositionAction;
        
        private System.Action<InputAction.CallbackContext> _meleeAttackInputAction;
        private System.Action<InputAction.CallbackContext> _startHeavyAttackInputAction;
        private System.Action<InputAction.CallbackContext> _stopHeavyAttackInputAction;
        private System.Action<InputAction.CallbackContext> _switchWeaponAction;
        private System.Action<InputAction.CallbackContext> _startDashAction;
        private System.Action<InputAction.CallbackContext> _stopDashAction;
        private System.Action<InputAction.CallbackContext> _startWeaponThrowAction;
        private System.Action<InputAction.CallbackContext> _stopWeaponThrowAction;
        private System.Action<InputAction.CallbackContext> _toggleAutoAttackAction;

        private bool _canTakeAttackInput;

        #region Initializers

        private void Awake()
        {
            _inputControls = new InputMaster();
            // keyboardMap = inp.actions.FindActionMap("Keyboard");
            // gamepadMap = playerInput.actions.FindActionMap("Gamepad");
            
            _canTakeAttackInput = true;
        }

        [Inject]
        private void InitializeDiReference(Camera cam, RunningDataScriptable runningDataScriptable, PlayerView playerView)
        {
            _camera = cam;
            _runningDataScriptable = runningDataScriptable;
            _playerView = playerView;
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            _meleeAttackInputAction = _ => MeleeAttackInput();
            _toggleAutoAttackAction = _ => ToggleAutoAttack();
            _switchWeaponAction = _ => SwitchWeapon();
            
            _startHeavyAttackInputAction = _ => StartHeavyAttackInput();
            _stopHeavyAttackInputAction = _ => StopHeavyAttackInput();
            
            _startDashAction = _ => StartDash();
            _stopDashAction = _ => StopDash();
            
            _startWeaponThrowAction = _ => StartWeaponThrow();
            _stopWeaponThrowAction = _ => StopWeaponThrow();
            
            
            _inputControls.PlayerControl.MeleeAttack.performed += _meleeAttackInputAction;
            _inputControls.PlayerControl.ToggleAutoAttack.performed += _toggleAutoAttackAction;
            _inputControls.PlayerControl.SwitchWeapon.performed += _switchWeaponAction;
            
            _inputControls.PlayerControl.HeavyAttack.performed += _startHeavyAttackInputAction;
            _inputControls.PlayerControl.HeavyAttack.canceled += _stopHeavyAttackInputAction;
            
            _inputControls.PlayerControl.Dash.performed += _startDashAction;
            _inputControls.PlayerControl.Dash.canceled += _stopDashAction;
            
            _inputControls.PlayerControl.WeaponThrow.performed += _startWeaponThrowAction;
            _inputControls.PlayerControl.WeaponThrow.canceled += _stopWeaponThrowAction;
        }
        
        private void UnSubscribeToActions()
        {
            _inputControls.PlayerControl.MeleeAttack.performed -= _meleeAttackInputAction;
            _inputControls.PlayerControl.SwitchWeapon.performed -= _switchWeaponAction;
            _inputControls.PlayerControl.ToggleAutoAttack.performed -= _toggleAutoAttackAction;
            
            _inputControls.PlayerControl.Dash.performed -= _startDashAction;
            _inputControls.PlayerControl.Dash.canceled -= _stopDashAction;
            
            _inputControls.PlayerControl.HeavyAttack.performed -= _startHeavyAttackInputAction;
            _inputControls.PlayerControl.HeavyAttack.canceled -= _stopHeavyAttackInputAction;
            
            _inputControls.PlayerControl.WeaponThrow.performed -= _startWeaponThrowAction;
            _inputControls.PlayerControl.WeaponThrow.canceled -= _stopWeaponThrowAction;
        }

        #endregion

        #region Enable and Disable

        private void OnEnable()
        {
            _inputControls.Enable();

            _mousePositionAction = _inputControls.PlayerControl.MousePosition;
            _mousePositionAction.Enable();
            SubscribeToActions();
        }

        private void OnDisable()
        {
            UnSubscribeToActions();
            _inputControls.Disable();
            _mousePositionAction.Disable();
        }

        #endregion

        #endregion

        #region Update

        private void Update()
        {
            _moveInput = _inputControls.PlayerControl.Movement.ReadValue<Vector2>();
            _signalBus.Fire(new PlayerMovementSignal(_moveInput));
            ReadMousePosition();
        }

        #endregion
        
        private void MeleeAttackInput()
        {
            if(!_canTakeAttackInput) return;
            _signalBus.Fire<LightAttackInputSignal>();
        }
        
        private void SwitchWeapon()
        {
            _signalBus.Fire<SwitchControlledWeaponSignal>();
        }

        #region Heavy Attack

        private void StartHeavyAttackInput()
        {
            _signalBus.Fire<StartHeavyAttackSignal>();
        }

        private void StopHeavyAttackInput()
        {
            _signalBus.Fire<StopHeavyAttackSignal>();
        }

        #endregion
        
        #region Dash

        private void StartDash()
        {
            _signalBus.Fire<CancelHeavyAttackSignal>();
            _signalBus.Fire<DashInputStartSignal>();
        }
        
        private void StopDash()
        {
            _signalBus.Fire<DashInputStopSignal>();
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
            var direction = (_playerView.gameObject.transform.position - mouseWorldPosition).normalized * -1;
            _runningDataScriptable.attackDirection = direction;
            _signalBus.Fire(new MouseMovementSignal(direction));
        }

        #endregion

        #region Weapon Throw

        private void StartWeaponThrow()
        {
            _signalBus.Fire<WeaponThrowStartSignal>();
        }
        private void StopWeaponThrow()
        {
            _signalBus.Fire<WeaponThrowStopSignal>();
        }

        #endregion

        #region Auto Attack

        private void ToggleAutoAttack()
        {
            _canTakeAttackInput = !_canTakeAttackInput;
            _signalBus.Fire<ToggleAutoAttackSignal>();
        }

        #endregion
    }
}