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

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     Debug.LogWarning("this method being called");
        //     if(!isBeingThrown) return;
        //     if (!other.gameObject.CompareTag("Enemy"))
        //     {
        //         Debug.LogWarning("other is not enemy");
        //         return;
        //     }
        //     var baseEnemy = other.gameObject.GetComponent<BaseEnemy>();
        //     if (baseEnemy == null)
        //     {
        //         Debug.LogWarning("base enemy is null");
        //         return;
        //     }
        //     Debug.LogWarning($"Enemy Hit! enemy damaged by {weaponData.damage}");
        //     baseEnemy.TakeDamage(weaponData.damage);
        //     baseEnemy.TakeKnockBack(transform, weaponData.knockBackStrength, weaponData.knockBackDuration);
        // }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.LogWarning($"Trigger entered by: {other.gameObject.name} at position {other.transform.position}");
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

            Debug.LogWarning($"Enemy Hit! enemy damaged by {weaponData.damage}");
            baseEnemy.TakeDamage(weaponData.damage);
            baseEnemy.TakeKnockBack(transform, weaponData.knockBackStrength, weaponData.knockBackDuration);
        }
    }
}