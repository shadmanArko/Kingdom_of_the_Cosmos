using System;
using Enemy.Manager;
using UnityEngine;

public class EnemyDamageValueSpawner : MonoBehaviour
{
    [SerializeField] private DamageValueCanvas damageValueCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        EnemyManager.OnEnemyTakeDeathDamage += ShowDamageValue;
    }

    private void ShowDamageValue(Vector3 position, float damageValue, Color damageColor)
    {
        DamageValueCanvas tempCanvas = Instantiate(damageValueCanvas, position, Quaternion.identity);
        tempCanvas.damageValueText.ShowDamageAnimation(damageValue, damageColor);
    }

    private void OnDisable()
    {
        EnemyManager.OnEnemyTakeDeathDamage -= ShowDamageValue;
    }
}
