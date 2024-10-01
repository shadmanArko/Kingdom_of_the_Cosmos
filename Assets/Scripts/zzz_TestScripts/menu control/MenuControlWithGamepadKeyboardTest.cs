using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace zzz_TestScripts.menu_control
{
    public class MenuControlWithGamepadKeyboardTest : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Button firstButton;
        void Start()
        {
            eventSystem = GetComponent<EventSystem>();
            eventSystem.firstSelectedGameObject = firstButton.gameObject;
        }
        
    }
}
