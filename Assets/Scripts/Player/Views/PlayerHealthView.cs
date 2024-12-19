using System;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Views
{
    public class PlayerHealthView : MonoBehaviour
    {
        public float maxHealthBar;
        public float maxShieldBar;
        
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider shieldSlider;
        
        #region Health Bar

        private float _healthBar;

        public float HealthBar
        {
            get => _healthBar;
            set
            {
                _healthBar = value;
                healthSlider.value = Mathf.Clamp(_healthBar, 0f, maxHealthBar);
            }
        }

        #endregion

        #region Shield Bar

        private float _shieldBar;
        public float ShieldBar
        {
            get => _shieldBar;
            set
            {
                _shieldBar = value;
                shieldSlider.value = Mathf.Clamp(_shieldBar, 0f, maxShieldBar);
            }
        }

        #endregion


        private void Awake()
        {
            
        }

        private void Start()
        {
            SubscribeToActions();
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
            
        }

        private void UnsubscribeToActions()
        {
            
        }

        #endregion

        private void OnDestroy()
        {
            UnsubscribeToActions();
        }
    }
}