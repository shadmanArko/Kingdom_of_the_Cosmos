using System.Collections.Generic;
using PlayerStats.Manager;
using UnityEngine;
using Zenject;

namespace PlayerStats
{
    public class StatModificationItemUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _iconImage;
        [SerializeField] private TMPro.TextMeshProUGUI _nameText;
        [SerializeField] private TMPro.TextMeshProUGUI _descriptionsText;
        [SerializeField] private UnityEngine.UI.Button _applyButton;

        private StatModificationConfig _config;
        private PlayerStatModificationManager _modificationManager;

        [Inject]
        public void Construct(PlayerStatModificationManager modificationManager)
        {
            _modificationManager = modificationManager;
        }

        public void Setup(
            string id, 
            Sprite icon, 
            Dictionary<string, float> modifications, 
            bool isPermanent, 
            float duration
        )
        {
            _config = new StatModificationConfig
            {
                Id = id,
                Icon = icon,
                StatModifications = modifications,
                IsPermanent = isPermanent,
                Duration = duration
            };

            _iconImage.sprite = icon;
            _nameText.text = id;
            _descriptionsText.text = BuildDescriptionText(modifications, isPermanent, duration);

            _applyButton.onClick.AddListener(ApplyModification);
        }

        private string BuildDescriptionText(
            Dictionary<string, float> modifications, 
            bool isPermanent, 
            float duration
        )
        {
            var descriptions = new List<string>();
            foreach (var mod in modifications)
            {
                descriptions.Add($"{mod.Key}: {(mod.Value > 0 ? "+" : "")}{mod.Value}");
            }
        
            var typeText = isPermanent ? "Permanent" : $"Temporary ({duration}s)";
            descriptions.Add($"Type: {typeText}");

            return string.Join("\n", descriptions);
        }

        private void ApplyModification()
        {
            _modificationManager.ApplyStatModification(_config);
        }
    }
}