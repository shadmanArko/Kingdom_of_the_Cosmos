using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementSystemUsingComputeShader : MonoBehaviour
{
    public ComputeShader enemyComputeShader;
    public GameObject player;
    public GameObject enemyPrefab;
    public int enemyCount = 10000;
    public float moveSpeed = 5f;

    private ComputeBuffer enemyBuffer;
    private Enemy[] enemies;
    private List<GameObject> enemyInstances;  // Pool of enemy GameObjects

    struct Enemy
    {
        public Vector2 position;
        public Vector2 velocity;
    }

    void Start()
    {
        // Initialize enemies
        enemies = new Enemy[enemyCount];
        for (int i = 0; i < enemyCount; i++)
        {
            enemies[i].position = new Vector2(Random.Range(-5000f, 5000f), Random.Range(-5000f, 5000f));
            enemies[i].velocity = Vector2.zero;
        }

        // Create compute buffer for enemy data
        enemyBuffer = new ComputeBuffer(enemyCount, sizeof(float) * 4);
        enemyBuffer.SetData(enemies);

        // Set buffer in the compute shader
        enemyComputeShader.SetBuffer(0, "enemyBuffer", enemyBuffer);
        enemyComputeShader.SetInt("enemyCount", enemyCount);  // Pass enemy count to shader

        // Instantiate enemy objects using object pooling
        enemyInstances = new List<GameObject>(enemyCount);
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyInstance = Instantiate(enemyPrefab, enemies[i].position, Quaternion.identity);
            enemyInstances.Add(enemyInstance);
        }
    }

    void Update()
    {
        // Pass player's position and deltaTime to the compute shader
        Vector2 playerPosition = player.transform.position;
        enemyComputeShader.SetVector("playerPosition", playerPosition);
        enemyComputeShader.SetFloat("deltaTime", Time.deltaTime);
        enemyComputeShader.SetFloat("moveSpeed", moveSpeed);

        // Dispatch compute shader with enough threads for all enemies
        int threadGroups = Mathf.CeilToInt(enemyCount / 256f);
        enemyComputeShader.Dispatch(0, threadGroups, 1, 1);

        // Retrieve updated positions from the GPU
        enemyBuffer.GetData(enemies);

        // Update enemy positions in the scene
        for (int i = 0; i < enemyCount; i++)
        {
            enemyInstances[i].transform.position = enemies[i].position;
        }
    }

    private void OnDestroy()
    {
        // Release the compute buffer to avoid memory leaks
        if (enemyBuffer != null)
        {
            enemyBuffer.Release();
        }
    }
}
