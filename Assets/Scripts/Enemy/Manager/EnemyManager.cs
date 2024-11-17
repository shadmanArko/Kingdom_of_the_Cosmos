using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Signals.BattleSceneSignals;
using Unity.Mathematics;
using Unity.VisualScripting;
using Zenject;
using IInitializable = Zenject.IInitializable;

public class EnemyManager : IInitializable, ITickable, IDisposable
{
    public static Action<int> EnemyCountUpdated;
   

    private readonly PlayerController _playerController;
    private readonly ComputeShader _enemyComputeShader;
    private readonly MeleeEnemyPool _meleeEnemyPool;
    private readonly SignalBus _signalBus;

    private Transform _playerTransform;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    private ComputeBuffer _computeBuffer;
    private ComputeBuffer _obstacleBuffer;
    private int _kernelIndex;
    private float _timeSinceLastCheck;
    private float enemySpawningInterval = 1f;
    private float nextEnemySpawnTime;
    private EnemyData[] enemyDataArray;
    public float moveSpeed = 4f;
    public float obstacleAvoidanceRadius = 1f;
    public float neighborAvoidanceRadius = 1f;
    public float collisionDistance = 1f;
    public float checkInterval = 0.1f;
    public float stucknessThreshold = 2f;
    

    public EnemyManager(PlayerController playerController,
        ComputeShader enemyComputeShader,
        MeleeEnemyPool meleeEnemyMeleeEnemyPool,
        SignalBus signalBus)
    {
        _playerController = playerController;
        _enemyComputeShader = enemyComputeShader;
        _meleeEnemyPool = meleeEnemyMeleeEnemyPool;
        _signalBus = signalBus;
        Debug.Log($"Enemy Manager constructor Started. signal bus found {_signalBus!= null}");

    }

    public void Initialize()
    {
        _playerTransform = _playerController.transform;
        nextEnemySpawnTime = enemySpawningInterval + Time.time;
        _signalBus.Subscribe<MeleeAttackSignal>(OnMeleeAttack);
        Debug.Log("Enemy Manager Started.");
        if (_enemyComputeShader != null)
        {
            _kernelIndex = _enemyComputeShader.FindKernel("CSMain");
        }
        InitializeObstacles();
    }

    

    private void CreateEnemyFromMeleeEnemyPool()
    {
        var meleeAttacker = new MeleeAttacker()
        {
            Damage = 15,
            Health = 100,
            IsAlive = 1,
            DistanceToPlayer = 9999f
        };
        _meleeEnemyPool.CreateMeleeEnemy(meleeAttacker);
    }
    public void Tick()
    {
        HandleKnockBack();
        HandleEnemySpawning();
        _activeEnemies = _meleeEnemyPool.activeEnemies;
        if (_activeEnemies.Count > 0)
        {
            UpdateBuffers();
        }
        
        if (_activeEnemies.Count == 0) return;
        
        UpdateEnemyPositions();
        for (int i = enemyDataArray.Length - 1; i >= 0; i--)
        {
            if (enemyDataArray[i].isAlive == 0)
            {
                //Todo: handle enemy death 
            }
            else
            {
                // _activeEnemies[i].GetComponent<>()
            }
        }
    }
    
    
    private List<KnockbackData> _enemiesBeingKnockedBack = new List<KnockbackData>();
    
    private void OnMeleeAttack()
    {
        var playerPos = _playerController.transform.position;
        var knockBackStrength = 10;
        var enemiesWithinArea = GetAllEnemiesWithinAttackArea(playerPos, new Vector2(playerPos.x+5f, playerPos.y+2.5f), new Vector2(playerPos.x+5f, playerPos.y-2.5f));
    
        foreach (var enemy in enemiesWithinArea)
        {
            StartKnockback(enemy.gameObject, playerPos, knockBackStrength);
        }
    }

    #region KnockBack
    private class KnockbackData
    {
        public GameObject Enemy;
        public Vector3 Direction;
        public float KnockbackStrength;
        public float ElapsedTime;
        public float Duration;
        public float StunDuration;
    }
    private void StartKnockback(GameObject enemy, Vector3 playerPos, float knockBackStrength)
    {
        var enemyTransform = enemy.transform;
        var direction = (enemyTransform.position - playerPos).normalized;
        
        _meleeEnemyPool.RemoveFromActiveEnemies(enemy);
        
        _enemiesBeingKnockedBack.Add(new KnockbackData
        {
            Enemy = enemy,
            Direction = direction,
            KnockbackStrength = knockBackStrength,
            ElapsedTime = 0f,
            Duration = 0.2f, // Knockback duration
            StunDuration = 0.2f // Knockback duration
        });
    }

    private void HandleKnockBack()
    {
        for (int i = _enemiesBeingKnockedBack.Count - 1; i >= 0; i--)
        {
            var knockback = _enemiesBeingKnockedBack[i];
            knockback.ElapsedTime += Time.deltaTime;

            if (knockback.ElapsedTime >= knockback.Duration)
            {
                // Knockback complete, add back to active enemies
               
                //wait for stun amount
                if (knockback.ElapsedTime >= knockback.Duration + knockback.StunDuration)
                {
                    
                    var meleeEnemy = knockback.Enemy.GetComponent<BaseEnemy>();
                    meleeEnemy.Velocity = 0;
                    var position = meleeEnemy.transform.position;
                    meleeEnemy.Position = new Vector2(position.x, position.y);
                    // meleeEnemy.SetMeleeAttackerStat(attackerStat);
                    _meleeEnemyPool.AddToActiveEnemies(knockback.Enemy);
                    _enemiesBeingKnockedBack.RemoveAt(i);}
            }
            else
            {
                // Continue knockback
                var enemyTransform = knockback.Enemy.transform;
                var enemyPos = enemyTransform.position;
                enemyPos += knockback.Direction * knockback.KnockbackStrength * Time.deltaTime;
                enemyTransform.position = enemyPos;
            }
        }
    }

    #endregion

    private IEnumerator KnockBackEnemy(GameObject enemy, Vector3 playerPos, float knockBackStrength)
    {
        var enemyTransform = enemy.transform;
        var direction = (enemyTransform.position - playerPos).normalized;
        float knockBackDuration = 0.2f; // Adjust this value to control knockback duration
        float elapsedTime = 0f;

        // Remove from active enemies at start of knockback
        _meleeEnemyPool.RemoveFromActiveEnemies(enemy);

        // Perform knockback over time
        while (elapsedTime < knockBackDuration)
        {
            var enemyPos = enemyTransform.position;
            enemyPos += direction * knockBackStrength * Time.deltaTime;
            enemyTransform.position = enemyPos;
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Add back to active enemies after knockback is complete
        _meleeEnemyPool.AddToActiveEnemies(enemy);
    }

    private List<BaseEnemy> GetAllEnemiesWithinAttackArea( Vector2 point1, Vector2 point2, Vector2 point3)
    {
        List<BaseEnemy> enemiesWithinArea = new List<BaseEnemy>();
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            var enemy = _activeEnemies[i];
            var meleeEnemy = enemy.GetComponent<BaseEnemy>();
            if (IsPointInTriangle(meleeEnemy.Position, point1, point2, point3))
            {
                enemiesWithinArea.Add(meleeEnemy);
                // Debug.Log($"Found Enemy Within Area for point:{enemyStat.Position}, triPoint1: { point1}, triPoint2: {point2}, triPoint3: {point3},");
            }
        }

        return enemiesWithinArea;
    }
    bool IsPointInTriangle(Vector2 point, Vector2 point1, Vector2 point2, Vector2 point3)
    {
        // Calculate the vectors from the point to each vertex
        Vector2 v0 = point3 - point1;
        Vector2 v1 = point2 - point1;
        Vector2 v2 = point - point1;

        // Calculate the cross products
        float cross00 = v0.x * v1.y - v0.y * v1.x;
        float cross01 = v0.x * v2.y - v0.y * v2.x;
        float cross11 = v1.x * v1.y - v1.y * v1.x;

        // Calculate the barycentric coordinates
        float denom = cross00;
        if (Mathf.Abs(denom) < 0.00001f)
            return false; // Avoid division by zero

        float u = cross01 / denom;
        float v = (v1.x * v2.y - v1.y * v2.x) / denom;

        // Check if the point is inside the triangle
        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }

    
    private void HandleEnemySpawning()
    {
        if (nextEnemySpawnTime < Time.time )
        {
            CreateEnemyFromMeleeEnemyPool();
            nextEnemySpawnTime = Time.time + enemySpawningInterval;
        }

        
    }

    private void UpdateEnemyPositions()
    {
        if (_computeBuffer == null || _obstacleBuffer == null || _activeEnemies.Count <= 0) return;

        // Update enemy positions in the buffer
        enemyDataArray = new EnemyData[_activeEnemies.Count];
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            Vector3 position = _activeEnemies[i].transform.position;
            var enemyStat = _activeEnemies[i].GetComponent<BaseEnemy>();
            
            enemyDataArray[i] = new EnemyData
            {
                position = enemyStat.Position,
                velocity = enemyStat.Velocity,
                stuckness = enemyStat.Stuckness, // Initialize stuckness
                damage = enemyStat.Damage,
                health = enemyStat.Health,
                isAlive = enemyStat.IsAlive ? 1: 0
            };
        }
        _computeBuffer.SetData(enemyDataArray);

        // Set compute shader parameters
        _enemyComputeShader.SetBuffer(_kernelIndex, "enemyBuffer", _computeBuffer);
        _enemyComputeShader.SetBuffer(_kernelIndex, "obstacleBuffer", _obstacleBuffer);
        _enemyComputeShader.SetVector("playerPosition", _playerTransform.position);
        _enemyComputeShader.SetFloat("deltaTime", Time.deltaTime);
        _enemyComputeShader.SetFloat("moveSpeed", moveSpeed);
        _enemyComputeShader.SetFloat("obstacleAvoidanceRadius", obstacleAvoidanceRadius);
        _enemyComputeShader.SetFloat("neighborAvoidanceRadius", neighborAvoidanceRadius);
        _enemyComputeShader.SetInt("enemyCount", _activeEnemies.Count);
        _enemyComputeShader.SetInt("obstacleCount", _obstacleBuffer.count);
        _enemyComputeShader.SetFloat("stucknessThreshold", stucknessThreshold);

        // Dispatch the compute shader
        int threadGroups = Mathf.CeilToInt(_activeEnemies.Count / 256f);
        _enemyComputeShader.Dispatch(_kernelIndex, threadGroups, 1, 1);

        // Get results back and update enemy positions
        _computeBuffer.GetData(enemyDataArray);
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            // _activeEnemies[i].transform.position = new Vector3(enemyDataArray[i].position.x, enemyDataArray[i].position.y, 0);
            var enemyData = enemyDataArray[i];
            var enemyStat = new MeleeAttacker();
            enemyStat.ApplyComputeData(enemyData);
            var enemy = _activeEnemies[i].GetComponent<BaseEnemy>();
            Debug.Log($"Sending move data from {enemy.transform.position} to {enemyData.position}");
            enemy.SetStat(enemyData);
            enemy.Move(new Vector2(enemyData.position.x, enemyData.position.y));
            // _activeEnemies[i].GetComponent<MeleeEnemy>().SetMeleeAttackerStat(enemyStat);
            if (enemyStat.DistanceToPlayer < 0.01f)
            {
                enemy.Attack(_playerController);
            }
        }
    }

    private void CheckEnemyCollisions()
    {
        Vector2 playerPosition = _playerTransform.position;
        float sqrCollisionDistance = collisionDistance * collisionDistance;

        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            Vector2 enemyPosition = _activeEnemies[i].transform.position;
            if ((playerPosition - enemyPosition).sqrMagnitude <= sqrCollisionDistance)
            {
                // Collision detected, return enemy to pool
                GameObject enemy = _activeEnemies[i];
                // _activeEnemies.RemoveAt(i);
                enemy.GetComponent<MeleeEnemy>().Attack(_playerController);
                // if (_meleeEnemyPool != null)
                // {
                //     _meleeEnemyPool.ReleaseEnemy(enemy);
                //     
                // }
                // else
                // {
                //     Debug.LogWarning("EnemySpawner not set. Destroying enemy instead.");
                //     Destroy(enemy);
                // }
            }
        }
    }

    private void InitializeObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        Vector2[] obstaclePositions = new Vector2[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstaclePositions[i] = obstacles[i].transform.position;
        }

        _obstacleBuffer = new ComputeBuffer(obstaclePositions.Length, sizeof(float) * 2);
        _obstacleBuffer.SetData(obstaclePositions);
    }

    public void AddEnemy(GameObject enemy)
    {
        _activeEnemies.Add(enemy);
        UpdateBuffers();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        _activeEnemies.Remove(enemy);
        UpdateBuffers();
    }
    
    private void UpdateBuffers()
    {
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        
        if (_activeEnemies.Count > 0)
        {
            _computeBuffer = new ComputeBuffer(_activeEnemies.Count, 36); // 2 for position, 2 for velocity, 1 for stuckness
        }
        else
        {
            _computeBuffer = null;
        }
        EnemyCountUpdated?.Invoke(_activeEnemies.Count);
    }

    private void OnDestroy()
    {
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        if (_obstacleBuffer != null)
        {
            _obstacleBuffer.Release();
        }
    }

    public void Dispose()
    {
        if (_computeBuffer != null)
        {
            _computeBuffer.Release();
        }
        if (_obstacleBuffer != null)
        {
            _obstacleBuffer.Release();
        }
    }
}