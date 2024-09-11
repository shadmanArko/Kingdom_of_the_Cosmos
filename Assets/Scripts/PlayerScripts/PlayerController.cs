using UnityEngine;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        
        [SerializeField] private Vector2 movement;
        
        [SerializeField] private Rigidbody2D rb;

        private void Start()
        {
            
        }

        public void Move(Vector2 direction)
        {
            movement = direction.normalized;
            rb.velocity = movement * moveSpeed;
        }

        public void Attack(Vector2 direction)
        {
            
        }
    }
}
