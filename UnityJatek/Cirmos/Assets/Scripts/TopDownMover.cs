using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputReader))]
public sealed class TopDownMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Range(0f, 20f)] private float moveSpeed = 4.0f;

    private Rigidbody2D rb;
    private PlayerInputReader input;
    private Vector2 desiredVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputReader>();

        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
  
    void Update()
    {
       
        desiredVelocity = input.MoveAxis * moveSpeed;
    }
    private void FixedUpdate()
    {
        Vector2 newPos = rb.position + desiredVelocity * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
    public void SetMoveSpeed(float newSpeed) => moveSpeed = Mathf.Max(0f, newSpeed); 
   
}
