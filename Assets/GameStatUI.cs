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
        Application.targetFrameRate = -1; // No frame rate limi
        EnemyManager.EnemyCountUpdated += EnemyCountUpdated;
    }

    private void EnemyCountUpdated(int obj)
    {
        enemyCountText.text = $"Current Enemies: {obj}";
    }

    private float timer = 0f;
    private float refreshInterval = 0.5f; // update every half second

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= refreshInterval)
        {
            int fps = (int)(1.0f / Time.smoothDeltaTime);
            fpsText.text = $"fps: {fps}";
            timer = 0f; // reset the timer
        }
    }

    private void OnDestroy()
    {
        EnemyManager.EnemyCountUpdated -= EnemyCountUpdated;

    }
}
