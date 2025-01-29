using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSystem.Views
{
    public class PlayerStatusUiView : MonoBehaviour
    {
        public float maxHealthBar;
        public float maxShieldBar;

        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider shieldSlider;
        [SerializeField] private Slider expSlider;

        [SerializeField] private Slider lightAttackSlider;
        [SerializeField] private Slider heavyAttackSlider;
        [SerializeField] private Slider primaryDashSlider;
        [SerializeField] private Slider secondaryDashSlider;

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
            LightAttackSliderValue = 100;
            HeavyAttackSliderValue = 100;
            PrimaryDashSliderValue = 100;
            SecondaryDashSliderValue = 100;
        }

        #region Subscribe and Unsubscribe

        private void SubscribeToActions()
        {
        }

        private void UnsubscribeToActions()
        {
        }

        #endregion

        #region Status Bar

        public float LightAttackSliderValue
        {
            set => lightAttackSlider.value = value;
        }

        public float HeavyAttackSliderValue
        {
            set => heavyAttackSlider.value = value;
        }

        public float PrimaryDashSliderValue
        {
            get => primaryDashSlider.value;
            set => primaryDashSlider.value = value;
        }

        public float SecondaryDashSliderValue
        {
            get => secondaryDashSlider.value;
            set => secondaryDashSlider.value = value;
        }

        #endregion

        private void OnDestroy()
        {
            UnsubscribeToActions();
        }

        public async void SetPrimarySliderCooldown(int timeLimit)
        {
            await PerformSliderCooldown(primaryDashSlider, timeLimit);
        }

        public async void SetSecondarySliderCooldown(int timeLimit)
        {
            await PerformSliderCooldown(secondaryDashSlider, timeLimit);
        }

        public async void SetLightAttackSliderCooldown(int timeLimit)
        {
            await PerformSliderCooldown(lightAttackSlider, timeLimit);
        }

        public async void SetHeavyAttackSliderCooldown(int timeLimit)
        {
            await PerformSliderCooldown(heavyAttackSlider, timeLimit);
        }


        private async Task PerformSliderCooldown(Slider slider, int interval)
        {
            var steps = 100;
            var stepDuration = interval / steps;

            for (var i = 0; i <= steps; i++)
            {
                slider.value = i;
                await Task.Delay(stepDuration);
            }
        }
    }
}