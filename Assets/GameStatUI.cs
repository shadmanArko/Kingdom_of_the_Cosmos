using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStatUI : MonoBehaviour
{
    
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI fpsText;
    // Start is called before the first frame update
    void Start()
    {
        EnemyManager.EnemyCountUpdated += EnemyCountUpdated;
    }

    private void EnemyCountUpdated(int obj)
    {
        enemyCountText.text = $"Current Enemies: {obj}";
    }

    // Update is called once per frame
    void Update()
    {
        fpsText.text = $"fps: {(int)(1.0f / Time.smoothDeltaTime)}";
    }

    private void OnDestroy()
    {
        EnemyManager.EnemyCountUpdated -= EnemyCountUpdated;

    }
}
