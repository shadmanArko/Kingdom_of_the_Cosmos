using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float3 centralPoint = new float3(0f, 0f, 0f);

        foreach (var (enemy, localTransform) in SystemAPI.Query<RefRW<Enemy>, RefRW<LocalTransform>>())
        {
            // Calculate direction toward the central point
            float3 direction = math.normalize(centralPoint - localTransform.ValueRW.Position);
            
            // Move enemy toward the central point
            localTransform.ValueRW.Position += direction * enemy.ValueRW.speed * deltaTime;
        }
    }
}