using UnityEngine;

public class ShieldedEnemy : EnemyBase
{
    public float Shield { get; set; }

    public override void PostComputeLogic()
    {
        float absorbedDamage = Mathf.Min(Shield, Damage);
        Shield -= absorbedDamage;
        Health -= (Damage - absorbedDamage);
    }
}
