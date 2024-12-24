using System;
using System.Collections.Generic;
using System.Linq;
using DBMS.GameData;
using DBMS.RunningData;
using Enemy.Models;
using Enemy.Services;
using PlayerSystem.Controllers;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Views;
using UnityEngine;
using Zenject;
using IInitializable = Zenject.IInitializable;

namespace Enemy.Manager
{
    public class EnemyManager : IInitializable, ITickable, IDisposable
    {
        public static Action<int> EnemyCountUpdated;
   

        private readonly PlayerController _playerController;
        private readonly PlayerView _playerView;
        private readonly ComputeShader _enemyComputeShader;
        private readonly MeleeEnemyPool _meleeEnemyPool;
        private readonly MeleeShieldedEnemyPool _meleeShieldedEnemyPool;
        private readonly RangedEnemyPool _rangedEnemyPool;
        private readonly ShamanEnemyPool _shamanEnemyPool;
        private readonly SignalBus _signalBus;

        private Transform _playerTransform;
        private List<BaseEnemy> _activeEnemies = new List<BaseEnemy>();
        private ComputeBuffer _computeBuffer;
        private ComputeBuffer _obstacleBuffer;
        private int _kernelIndex;
        private float _timeSinceLastCheck;
        private float enemySpawningInterval = 1f;
        private float nextEnemySpawnTime;
        private EnemyData[] enemyDataArray;
        public float moveSpeed = 0.25f;
        public float obstacleAvoidanceRadius = 1f;
        public float neighborAvoidanceRadius = 1f;
        public float collisionDistance = 1f;
        public float checkInterval = 0.1f;
        public float stucknessThreshold = 2f;
        private GameDataScriptable _gameDataScriptable;
        private RunningDataScriptable _runningDataScriptable;
        private Enemies _enemies;

        public EnemyManager(PlayerController playerController,
            ComputeShader enemyComputeShader,
            MeleeEnemyPool meleeEnemyMeleeEnemyPool,
            MeleeShieldedEnemyPool meleeShieldedEnemyPool,
            RangedEnemyPool rangedEnemyPool,
            ShamanEnemyPool shamanEnemyPool,
            GameDataScriptable gameDataScriptable,
            RunningDataScriptable runningDataScriptable,
            SignalBus signalBus,
            PlayerView playerView)
        {
            _playerController = playerController;
            _enemyComputeShader = enemyComputeShader;
            _meleeEnemyPool = meleeEnemyMeleeEnemyPool;
            _rangedEnemyPool = rangedEnemyPool;
            _shamanEnemyPool = shamanEnemyPool;
            _meleeShieldedEnemyPool = meleeShieldedEnemyPool;
            _gameDataScriptable = gameDataScriptable;
            _runningDataScriptable = runningDataScriptable;
            _signalBus = signalBus;
            _playerView = playerView;
            Debug.Log($"Enemy Manager constructor Started. signal bus found {_signalBus!= null}");

        }

        public void Initialize()
        {
            _playerTransform = _playerView.transform;
            nextEnemySpawnTime = enemySpawningInterval + Time.time;
            _enemies = _gameDataScriptable.gameData.enemies;
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
            var meleeEnemyData = _enemies.MeleeEnemyDatas.FirstOrDefault(data => data.Id == "1");
            _meleeEnemyPool.CreateMeleeEnemy(meleeEnemyData);
        }
        private void CreateEnemyFromMeleeShieldedEnemyPool()
        {
            var meleeShieldedEnemyData = _enemies.MeleeShieldedEnemyDatas.FirstOrDefault(data => data.Id == "1");
            _meleeShieldedEnemyPool.CreateMeleeEnemy(meleeShieldedEnemyData);
        }
        private void CreateEnemyFromRangedEnemyPool()
        {
            var rangedEnemyData = _enemies.RangedEnemyDatas.FirstOrDefault(data => data.Id == "1");
            _rangedEnemyPool.CreateRangeEnemy(rangedEnemyData);
        }

        private void CreateEnemyFromShamanEnemyPool()
        {
            var shamanEnemyData = _enemies.ShamanEnemyDatas.FirstOrDefault(data => data.Id == "1");
            _shamanEnemyPool.CreateShamanEnemy(shamanEnemyData);
        }
        public void Tick()
        {
            HandleKnockBack();
            HandleEnemySpawning();
            _activeEnemies = _meleeEnemyPool.activeEnemies.Concat(_meleeShieldedEnemyPool.activeEnemies).ToList();
            _activeEnemies = _activeEnemies.Concat(_rangedEnemyPool.activeEnemies).ToList();
            _activeEnemies = _activeEnemies.Concat(_shamanEnemyPool.activeEnemies).ToList();
            if (_activeEnemies.Count > 0)
            {
                UpdateBuffers();
            }
        
            if (_activeEnemies.Count == 0) return;
        
            // UpdateEnemyPositions();
            UpdateEnemyPositionsWithoutShader();
        
        }

        private void UpdateEnemyPositionsWithoutShader()
        {
            var closestEnemyToPlayer = _activeEnemies[0];
            foreach (var enemy in _activeEnemies)
            {
                enemy.MoveTowardsTarget(_playerTransform);
            
                if (enemy.DistanceToPlayer < closestEnemyToPlayer.DistanceToPlayer)
                {
                    closestEnemyToPlayer = enemy;
                }
            }

            _runningDataScriptable.closestEnemyToPlayer = closestEnemyToPlayer.transform.position;
        }


        private List<KnockbackData> _enemiesBeingKnockedBack = new List<KnockbackData>();
    
        private void OnMeleeAttack(MeleeAttackSignal meleeAttackSignal)
        {
            Debug.Log("Melee Attack occured");
            var playerPos = _playerView.transform.position;
            var knockBackStrength = meleeAttackSignal.weaponData.knockBackStrength;
            var damageValue = meleeAttackSignal.weaponData.damage;
            var p0 = _runningDataScriptable.attackAnglePoints[0];
            var p1 = _runningDataScriptable.attackAnglePoints[1];
            var p2 =  _runningDataScriptable.attackAnglePoints[2];
            var enemiesWithinArea = GetAllEnemiesWithinAttackArea(p0, p1, p2);
            Debug.Log($"player pos:{playerPos}, p0: {p0}, p1: {p1}, p2: {p2} enemiesWithinArea: {enemiesWithinArea.Count}");
    
            foreach (var enemy in enemiesWithinArea)
            {
                enemy.TakeDamage(damageValue);
                if (!enemy.IsAlive)
                {
                    ReleaseEnemy(enemy);
                }else{
                    // StartKnockback(enemy, playerPos, knockBackStrength);
                    enemy.TakeKnockBack(_playerTransform);
                }
            }
        }

        private void ReleaseEnemy(BaseEnemy enemy)
        {
            if (enemy.GetComponent<MeleeShieldedEnemy>())
            {
                _meleeShieldedEnemyPool.ReleaseEnemy(enemy);
            }if (enemy.GetComponent<MeleeEnemy>())
            {
                _meleeEnemyPool.ReleaseEnemy(enemy);
            }
            if (enemy.GetComponent<RangedEnemy>())
            {
                _rangedEnemyPool.ReleaseEnemy(enemy);
            }
            if (enemy.GetComponent<ShamanEnemy>())
            {
                _shamanEnemyPool.ReleaseEnemy(enemy);
            }
        }


        #region KnockBack
        private class KnockbackData
        {
            public BaseEnemy Enemy;
            public Vector3 Direction;
            public float KnockbackStrength;
            public float ElapsedTime;
            public float Duration;
            public float StunDuration;
        }
        private void StartKnockback(BaseEnemy enemy, Vector3 playerPos, float knockBackStrength)
        {
            var enemyTransform = enemy.transform;
            var direction = (enemyTransform.position - playerPos).normalized;
        
            RemoveFromActiveEnemies(enemy);
        
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
        
        private void RemoveFromActiveEnemies(BaseEnemy enemy)
        {
            if (enemy.GetComponent<MeleeShieldedEnemy>())
            {
                _meleeShieldedEnemyPool.RemoveFromActiveEnemies(enemy);
            }

            if (enemy.GetComponent<MeleeEnemy>())
            {
                _meleeEnemyPool.RemoveFromActiveEnemies(enemy);
            }
            if (enemy.GetComponent<RangedEnemy>())
            {
                _rangedEnemyPool.RemoveFromActiveEnemies(enemy);
            }
            if (enemy.GetComponent<ShamanEnemy>())
            {
                _shamanEnemyPool.RemoveFromActiveEnemies(enemy);
            }
        }
        private void AddToActiveEnemies(BaseEnemy enemy)
        {
            if (enemy.GetComponent<MeleeShieldedEnemy>())
            {
                _meleeShieldedEnemyPool.AddToActiveEnemies(enemy);
            }

            if (enemy.GetComponent<MeleeEnemy>())
            {
                _meleeEnemyPool.AddToActiveEnemies(enemy);
            }
            if (enemy.GetComponent<RangedEnemy>())
            {
                _rangedEnemyPool.AddToActiveEnemies(enemy);
            }
            if (enemy.GetComponent<ShamanEnemy>())
            {
                _shamanEnemyPool.AddToActiveEnemies(enemy);
            }
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
                    
                        var meleeEnemy = knockback.Enemy;
                        meleeEnemy.Velocity = 0;
                        var position = meleeEnemy.transform.position;
                        meleeEnemy.Position = new Vector2(position.x, position.y);
                        // meleeEnemy.SetMeleeAttackerStat(attackerStat);
                        AddToActiveEnemies(knockback.Enemy);
                        _enemiesBeingKnockedBack.RemoveAt(i);}
                }
                else
                {
                    // Continue knockback
                    var enemyTransform = knockback.Enemy.transform;
                    var enemyPos = enemyTransform.position;
                    enemyPos += knockback.Direction * (knockback.KnockbackStrength * Time.deltaTime);
                    enemyTransform.position = enemyPos;
                }
            }
        }

        #endregion
    

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

            // Calculate dot products
            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            // Calculate barycentric coordinates
            float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }

        private int _numberOfMeleeEnemies = 20;
        private int _numberOfShamanEnemies = 1;
        private int _numberOfShieldedMeleeEnemies = 3;
        private int _countOfMeleeEnemies;
        private int _countOfShieldedMeleeEnemies;
        private int _countOfShamanEnemies;
        private void HandleEnemySpawning()
        {
        
            if (nextEnemySpawnTime < Time.time )
            {
                if (_countOfShamanEnemies >= _numberOfShamanEnemies)
                {
                    CreateEnemyFromRangedEnemyPool();
                    // CreateEnemyFromMeleeEnemyPool();
                }
                else if (_countOfShieldedMeleeEnemies >= _numberOfShieldedMeleeEnemies)
                {
                    _countOfShamanEnemies++;
                    CreateEnemyFromShamanEnemyPool();
                }
                else if (_countOfMeleeEnemies >= _numberOfMeleeEnemies)
                {
                    _countOfShieldedMeleeEnemies++;
                    CreateEnemyFromMeleeShieldedEnemyPool();
                }
                else
                {
                    _countOfMeleeEnemies++;
                    CreateEnemyFromMeleeEnemyPool();
                }
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
                var enemy = _activeEnemies[i];
            
                enemyDataArray[i] = new EnemyData
                {
                    position = position,
                    velocity = enemy.Velocity,
                    stuckness = enemy.Stuckness, // Initialize stuckness
                    damage = enemy.Damage,
                    isAlive = enemy.IsAlive ? 1: 0,
                    minDistanceToPlayer = enemy.MinDistanceToPlayer
                };
                // Debug.Log($"Sending health data  {enemy.Health}");
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
                var enemy = _activeEnemies[i];
                // Debug.Log($"Receiving health data  {enemy.Health}");
                enemy.SetStat(enemyData);
                enemy.Move(new Vector2(enemyData.position.x, enemyData.position.y));
                // _activeEnemies[i].GetComponent<MeleeEnemy>().SetMeleeAttackerStat(enemyStat);
                if (enemy.DistanceToPlayer < 2f)
                {
                    enemy.Attack(_playerView);
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
                    BaseEnemy enemy = _activeEnemies[i];
                    // _activeEnemies.RemoveAt(i);
                    enemy.Attack(_playerView);
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
            EnemyCountUpdated?.Invoke(_activeEnemies.Count + _enemiesBeingKnockedBack.Count);
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
}