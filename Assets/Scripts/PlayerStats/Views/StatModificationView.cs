using System.Collections.Generic;
using UnityEngine;

namespace PlayerStats
{
    public class StatModificationView : MonoBehaviour
    {
        [SerializeField] private Transform _modificationContainer;
        [SerializeField] private GameObject _modificationItemPrefab;

        public void DisplayModifications(List<StatModificationConfig> modifications)
        {
            // Clear existing modifications
            foreach (Transform child in _modificationContainer)
            {
                Destroy(child.gameObject);
            }

            // Create UI elements for each modification
            foreach (var config in modifications)
            {
                var modItem = Instantiate(_modificationItemPrefab, _modificationContainer);
                var modItemComponent = modItem.GetComponent<StatModificationItemUI>();
            
                modItemComponent.Setup(
                    config.Id, 
                    config.Icon, 
                    config.StatModifications, 
                    config.IsPermanent, 
                    config.Duration
                );
            }
        }
    }
}