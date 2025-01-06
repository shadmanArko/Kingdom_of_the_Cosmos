using Enemy.Models;
using UnityEngine;
using WeaponSystem.Models;

namespace PlayerSystem.Services
{
    public class ThrowableWeaponView : MonoBehaviour
    {
        public GameObject throwable;
        public SpriteRenderer weaponSpriteRend;
        public Rigidbody2D rb;

        public WeaponData weaponData;
        public bool isBeingThrown;

        private void Start()
        {
            isBeingThrown = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!isBeingThrown)
            {
                Debug.LogWarning("Weapon is not being thrown, ignoring collision");
                return;
            }
            if (!other.gameObject.CompareTag("Enemy"))
            {
                Debug.LogWarning($"Object tagged as: {other.gameObject.tag}");
                return;
            }
    
            var baseEnemy = other.gameObject.GetComponent<BaseEnemy>();
            if (baseEnemy == null)
            {
                Debug.LogWarning($"No BaseEnemy component on {other.gameObject.name}");
                return;
            }
            
            baseEnemy.TakeDamage(weaponData.damage);
            baseEnemy.TakeKnockBack(transform, weaponData.knockBackDuration, weaponData.knockBackStrength);
        }
    }
}