using ObjectPool;
using UnityEngine;

namespace Projectiles
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rBody;
        
        public float speed = 10f;
        public float lifetime = 2f;

        public Vector2 direction;

        [SerializeField] private float lifetimeCounter;
        private BulletPool _pool;

        [SerializeField] private bool isInitialized = false;
        
        private void Start()
        {
            lifetimeCounter = lifetime;
        }

        private void FixedUpdate()
        {
            if (!isInitialized) return;
            var moveSpeed = direction.normalized * (speed * Time.fixedDeltaTime);
            rBody.velocity =  moveSpeed;
            
            if (lifetimeCounter <= 0)
            {
                ReturnToPool();
                lifetimeCounter = 0;
            }
            else
            {
                lifetimeCounter -= Time.fixedDeltaTime;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Handle collision logic here
            if(collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Wall"))
                ReturnToPool();
        }

        public void Initialize(BulletPool tempPool, Vector2 dir)
        {
            _pool = tempPool;
            direction = dir;
            lifetimeCounter = lifetime;
            isInitialized = true;
            
        }

        private void ReturnToPool()
        {
            _pool.ReturnObject(gameObject);
            isInitialized = false;
        }
    }
}