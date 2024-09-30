using UnityEngine;
using UnityEngine.Pool;
using TestScripts.WeaponsTest;

namespace ObjectPoolScripts
{
    public class Ability : MonoBehaviour
    {
        private ObjectPool<Ability> _abilityPool;

        [SerializeField] private AttackBase attackBase;
        
        public void SetPool(ObjectPool<Ability> pool)
        {
            _abilityPool = pool;
        }

        public void Activate(AttackBase attackBaseType)
        {
            attackBase = attackBaseType;
            if(attackBaseType == null) Debug.Log("attack base is null");
            attackBase.Attack();
        }
    }

}
