using System.Collections.Generic;
using ObjectPool;
using ObjectPoolScripts;
using UnityEngine;

namespace TestScripts.WeaponsTest
{
    public abstract class AttackBase
    {
        
        public abstract void Attack(BulletPoolingManager manager, Vector2 attackerPos, Vector2 mousePos);
        
    }
}
