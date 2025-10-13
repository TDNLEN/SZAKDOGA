using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    public Transform target;              // ha üres, Start-ban megkeressük a "Player"-t
    public float moveSpeed = 2.5f;
    public float stoppingDistance = 0.3f;

    [Header("Animator (opcionális)")]
    public Animator animator;             // ha van animator a zombin
    public string speedParam = "Speed";   // float
    public string movingBoolParam = "";   // ha boolt használsz pl. "IsMoving"

    private Rigidbody2D rb;
    private Vector2 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

    }

    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 pos = rb.position;
        Vector2 tgt = target.position;
        Vector2 toTarget = tgt - pos;
        float dist = toTarget.magnitude;


        if (dist > stoppingDistance)
        {
            Vector2 dir = toTarget.normalized;
            velocity = dir * moveSpeed;
            rb.MovePosition(pos + velocity * Time.fixedDeltaTime);
        }
        else
        {
            velocity = Vector2.zero;
        }

        if (velocity.x != 0f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (velocity.x > 0 ? 1 : -1);
            transform.localScale = s;
        }

      
    }
}
