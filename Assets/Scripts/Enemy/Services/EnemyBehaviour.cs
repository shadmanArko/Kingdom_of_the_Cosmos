using UnityEngine;
using UnityEngine.Pool;

public class EnemyBehavior : MonoBehaviour
{
    private IObjectPool<GameObject> pool;
    private MeleeEnemyPool _pool;
    private Vector3 initialDirection;

    public void SetPool(IObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }

    public void SetSpawner(MeleeEnemyPool meleeEnemyPool)
    {
        _pool = meleeEnemyPool;
    }

    public void SetInitialDirection(Vector3 direction)
    {
        initialDirection = direction;
    }

    private void OnEnable()
    {
        // Apply a small initial movement when the enemy is enabled
        transform.position += initialDirection * 0.1f;
    }

    
}