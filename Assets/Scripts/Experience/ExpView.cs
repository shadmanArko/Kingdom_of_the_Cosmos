using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Experience
{
    public class ExpView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _level;
        
        [SerializeField] private TMP_Text _collectedExp;
        [SerializeField] private TMP_Text _maxExp;
        
        [SerializeField] private Slider _expSlider;

        public string Level
        {
            set => _level.SetText(value);
        }

        public string CollectedExp
        {
            set => _collectedExp.SetText(value);
        }

        public string MaxExp
        {
            set => _maxExp.SetText(value);
        }

        public float ExpSliderValueInPercentage
        {
            set => _expSlider.value = value;
        }
    }
}