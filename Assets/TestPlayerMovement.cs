using Enemy.Services;
using UnityEngine;
using UnityEngine.Serialization;

public class TestPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    [FormerlySerializedAs("spawner")] [SerializeField] private MeleeEnemyPool pool;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input axes
        float moveHorizontal = Input.GetAxisRaw("Horizontal"); // A & D or Left & Right arrows
        float moveVertical = Input.GetAxisRaw("Vertical");     // W & S or Up & Down arrows

        // Calculate movement vector
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        // Normalize to prevent faster diagonal movement
        movement = movement.normalized * moveSpeed;

        // Apply movement to the rigidbody
        rb.linearVelocity = movement;
    }
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Enemy"))
    //     {
    //         // Enemy touched the player, return it to the pool
    //         if (spawner != null)
    //         {
    //             spawner.ReleaseEnemy(other.gameObject);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("EnemySpawner not set. Destroying enemy instead.");
    //             Destroy(other.gameObject);
    //         }
    //     }
    // }
}