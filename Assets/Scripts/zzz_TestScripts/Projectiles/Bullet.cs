using UnityEngine;
using UnityEngine.Pool;

namespace zzz_TestScripts.Projectiles
{
    public class Bullet : MonoBehaviour
    {
        private ObjectPool<Bullet> _pool;
        
        public float speed = 10f;
        public float lifetime = 2f;
        public Vector2 direction;

        [SerializeField] private Rigidbody2D rBody;
        [SerializeField] private float lifetimeCounter;
        [SerializeField] private bool isInitialized = false;
        
        private void Start()
        {
            lifetimeCounter = lifetime;
        }

        private void FixedUpdate()
        {
            if (!isInitialized) return;
            var moveSpeed = direction.normalized * (speed * Time.fixedDeltaTime);
            rBody.linearVelocity =  moveSpeed;
            
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
            if(collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Wall"))
                ReturnToPool();
        }

        public void Initialize(ObjectPool<Bullet> tempPool, Vector2 dir)
        {
            _pool = tempPool;
            direction = dir;
            lifetimeCounter = lifetime;
            isInitialized = true;
        }

        private void ReturnToPool()
        {
            _pool.Release(this);
            isInitialized = false;
        }


        public void SetPool(ObjectPool<Bullet> pool)
        {
            _pool = pool;
        }
    }
}