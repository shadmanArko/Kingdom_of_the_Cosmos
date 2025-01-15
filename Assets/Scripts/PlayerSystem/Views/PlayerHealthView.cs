using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSystem.Views
{
    public class PlayerHealthView : MonoBehaviour
    {
        public float maxHealthBar;
        public float maxShieldBar;
        
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider shieldSlider;
        [SerializeField] private Slider expSlider;
        
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

        #region Experience Bar

        [SerializeField] private TMP_Text level;
        
        [SerializeField] private TMP_Text collectedExp;
        [SerializeField] private TMP_Text maxExp;

        public string Level
        {
            set => level.SetText($"Level: {value}");
        }

        public string CollectedExp
        {
            set => collectedExp.SetText($"{value} /");
        }

        public string MaxExp
        {
            set => maxExp.SetText(value);
        }

        public float ExpSliderValueInPercentage
        {
            set => expSlider.value = value;
        }

        #endregion

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