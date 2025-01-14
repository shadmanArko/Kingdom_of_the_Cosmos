using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experience
{
    public class ExpView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _level;
        [SerializeField] private TMP_Text _exp;
        [SerializeField] private Slider _expSlider;

        public string Level
        {
            set => _level.SetText(value);
        }

        public string Exp
        {
            set => _exp.SetText(value);
        }

        public float ExpSliderValueInPercentage
        {
            set => _expSlider.value = value;
        }
    }
}