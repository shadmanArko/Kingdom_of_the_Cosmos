using UnityEngine;
using UnityEngine.Pool;
using ObjectPool;
using PlayerScripts;
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

        public void Activate(BulletPoolingManager bulletPoolingManager, PlayerController playerController, AttackBase attackBaseType, Vector2 mousePos)
        {
            attackBase = attackBaseType;
            if(attackBaseType == null) Debug.Log("attack base is null");
            attackBase.Attack(bulletPoolingManager, playerController.gameObject.transform.position, mousePos);
        }
    }

}
