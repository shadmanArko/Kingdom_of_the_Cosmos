using Unity.Burst;
using Unity.Entities;

namespace zzz_TestScripts
{
    [BurstCompile]
    public partial struct EnemySpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        // [BurstCompile]
        // public void OnUpdate(ref SystemState state)
        // {
        //     var enemyPrefab = SystemAPI.GetSingleton<EnemyPrefab>();
        //     int enemyCount = 1000;
        //     float3 centralPoint = new float3(0f, 0f, 0f);
        //     
        //     // Create a NativeArray for random positions
        //     NativeArray<float3> randomPositions = new NativeArray<float3>(enemyCount, Allocator.TempJob);
        //
        //     Random random = new Random(1); // Seed for random numbers
        //     
        //     for (int i = 0; i < enemyCount; i++)
        //     {
        //         randomPositions[i] = random.NextFloat3(new float3(-50f, 0f, -50f), new float3(50f, 0f, 50f));
        //     }
        //
        //     // Spawn enemies
        //     for (int i = 0; i < enemyCount; i++)
        //     {
        //         var entity = state.EntityManager.Instantiate(enemyPrefab.Entity);
        //         state.EntityManager.SetComponentData(entity, new LocalTransform
        //         {
        //             Position = randomPositions[i],
        //             Scale = 1f
        //         });
        //         state.EntityManager.SetComponentData(entity, new Enemy
        //         {
        //             position = randomPositions[i],
        //             speed = random.NextFloat(1f, 3f)
        //         });
        //     }
        //
        //     randomPositions.Dispose();
        // }
    }
}