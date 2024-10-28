
public class MeleeAttacker : EnemyBase
{
    public override void PostComputeLogic()
    {
        Health -= Damage;
    }
}
