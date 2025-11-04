using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SkeletonRangedAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                // ha üres, Start-ban keresi a "Player"-t

    [Header("Movement")]
    public float moveSpeed = 2.0f;
    public float preferredDistance = 6f;    // ennyire szeret lenni a playertől
    public float distanceTolerance = 0.5f;  // ekkora sávban marad

    [Header("Attack")]
    public GameObject projectilePrefab;     // SkeletonProjectile prefab
    public Transform shootPoint;            // ahonnan kilövi (pl. kéz csont)
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

        // ha túl messze van → kicsit közelebb megy
        if (dist > preferredDistance + distanceTolerance)
        {
            Vector2 dir = toTarget.normalized;
            velocity = dir * moveSpeed;
        }
        // ha túl közel került → hátrál
        else if (dist < preferredDistance - distanceTolerance)
        {
            Vector2 dir = (-toTarget).normalized;
            velocity = dir * moveSpeed;
        }
        // ha jó távolságban van → áll, csak lőni fog

        rb.MovePosition(pos + velocity * Time.fixedDeltaTime);

        // sprite flip
        if (toTarget.x != 0f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (toTarget.x > 0 ? 1 : -1);
            transform.localScale = s;
        }

        // animator move flag
        if (animator != null && !string.IsNullOrEmpty(isMovingBool))
            animator.SetBool(isMovingBool, velocity.sqrMagnitude > 0.001f);

        // támadás logika (Update helyett itt is jó, dist adott)
        TryShoot(toTarget, dist);
    }

    private void TryShoot(Vector2 toTarget, float dist)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        // csak akkor lőjön, ha kb. a preferált távolságon belül van látótávban
        if (dist > preferredDistance + 1.0f) return; // túl messze

        if (Time.time < nextShootTime) return;
        nextShootTime = Time.time + attackCooldown;

        // anim trigger
        if (animator != null && !string.IsNullOrEmpty(shootTrigger))
            animator.SetTrigger(shootTrigger);

        // maga a lövés
        Vector2 dir = toTarget.normalized;
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        var ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.Init(dir);
    }
}
