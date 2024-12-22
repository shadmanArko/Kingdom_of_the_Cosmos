using System;
using Enemy.Models;
using UnityEngine;
using WeaponSystem.Models;

namespace Player.Services
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
            if(!isBeingThrown) return;
            if(!other.gameObject.CompareTag("Enemy")) return;
            var baseEnemy = other.gameObject.GetComponent<BaseEnemy>();
            if(baseEnemy == null) return;
            baseEnemy.TakeDamage(weaponData.damage);
            baseEnemy.TakeKnockBack(transform, weaponData.knockBackStrength, weaponData.knockBackDuration);
        }
    }
}