using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;

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
        rb.velocity = movement;
    }
}