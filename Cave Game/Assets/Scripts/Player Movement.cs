using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed of the player

    private Vector2 movementInput;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
    }

    void Update()
    {
        // Get movement input from player (WASD or Arrow keys)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Normalize movement vector to prevent diagonal speed boost
        if (movementInput.magnitude > 1)
        {
            movementInput.Normalize();
        }
    }

    void FixedUpdate()
    {
        // Apply movement smoothly in the FixedUpdate for physics updates
        rb.linearVelocity = movementInput * moveSpeed;
    }
}
