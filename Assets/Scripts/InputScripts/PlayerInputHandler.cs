using PlayerScripts;
using UnityEngine;

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
        }
    }
}