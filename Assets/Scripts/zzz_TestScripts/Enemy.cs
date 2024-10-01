using Unity.Entities;
using Unity.Mathematics;

public struct Enemy : IComponentData
{
    public float3 position;
    public float speed;
}