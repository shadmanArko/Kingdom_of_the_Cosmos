using Unity.Mathematics;

public abstract class EnemyBase
{
    public float2 Position { get; set; }
    public float2 Velocity { get; set; }
    public float Health { get; set; }
    public float Stuckness { get; set; }
    public float Damage { get; set; }
    public int IsAlive { get; set; }

    // Method to apply common data from compute shader
    public virtual void ApplyComputeData(EnemyData data)
    {
        Position = data.position;
        Velocity = data.velocity;
        Stuckness = data.stuckness;
        Damage = data.damage;
        Health = data.health;
        IsAlive = data.isAlive;
    }

    // Abstract method for custom logic per enemy type
    public abstract void PostComputeLogic();
}