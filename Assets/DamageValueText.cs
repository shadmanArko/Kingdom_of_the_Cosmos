using TMPro;
using UnityEngine;

public class DamageValueText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageValueText;
    [SerializeField] private Animator animator;

    public void ShowDamageAnimation(float damageValue, Color damageColor)
    {
        damageValueText.color = damageColor;
        damageValueText.text = damageValue.ToString("0");
        // animator.Play($"DamageValueTextAnimation");
        animator.SetTrigger($"Damage");
    }
}
