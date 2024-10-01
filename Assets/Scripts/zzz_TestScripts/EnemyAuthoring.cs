using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;

    // Baking: This runs when the scene is converted
    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyPrefab
            {
                Entity = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

// This struct will hold a reference to the enemy prefab entity
public struct EnemyPrefab : IComponentData
{
    public Entity Entity;
}