using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SkeletonRangedAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;               

    [Header("Movement")]
    public float moveSpeed = 2.0f;
    public float preferredDistance = 6f;    
    public float distanceTolerance = 0.5f;  

    [Header("Attack")]
    public GameObject projectilePrefab;    
    public Transform shootPoint;           
    public float attackCooldown = 1.5f;

    [Header("Animator (opcionális)")]
    public Animator animator;
    public string isMovingBool = "IsMoving";
    public string shootTrigger = "Shoot";

    private Rigidbody2D rb;
    private float nextShootTime = 0f;
    private Vector2 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 pos = rb.position;
        Vector2 tgt = target.position;
        Vector2 toTarget = tgt - pos;
        float dist = toTarget.magnitude;

        velocity = Vector2.zero;

        if (dist > preferredDistance + distanceTolerance)
        {
            Vector2 dir = toTarget.normalized;
            velocity = dir * moveSpeed;
        }
        else if (dist < preferredDistance - distanceTolerance)
        {
            Vector2 dir = (-toTarget).normalized;
            velocity = dir * moveSpeed;
        }

        rb.MovePosition(pos + velocity * Time.fixedDeltaTime);

        if (toTarget.x != 0f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (toTarget.x > 0 ? 1 : -1);
            transform.localScale = s;
        }

        if (animator != null && !string.IsNullOrEmpty(isMovingBool))
            animator.SetBool(isMovingBool, velocity.sqrMagnitude > 0.001f);

        TryShoot(toTarget, dist);
    }

    private void TryShoot(Vector2 toTarget, float dist)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        if (dist > preferredDistance + 1.0f) return; // túl messze

        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + attackCooldown;

        if (animator != null && !string.IsNullOrEmpty(shootTrigger))
            animator.SetTrigger(shootTrigger);

        Vector2 dir = toTarget.normalized;
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        var ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.Init(dir);
    }
}
