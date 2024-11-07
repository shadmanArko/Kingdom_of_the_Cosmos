using UnityEngine;
using UnityEngine.Serialization;

namespace RicochetWeaponSystem
{
    public class DummyEnemy : MonoBehaviour
    {
        public float CollisionRadius = 0.5f;
        [SerializeField] private bool isShielded = false;
        [SerializeField] private float shieldAngle = 0f; // Angle in degrees for shield direction
    
        public bool IsShielded => isShielded;
        public Vector2 ShieldNormal => Quaternion.Euler(0, 0, shieldAngle) * Vector2.right;
    }

}