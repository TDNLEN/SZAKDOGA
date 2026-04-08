using UnityEngine;

public sealed class PlayerInputReader : MonoBehaviour
{
    [SerializeField] public Animator animator;

    [Header("Walk Audio")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    [Range(0f, 1f)] public float walkVolume = 1f;
    public float walkInterval = 0.5f;

    public Vector2 MoveAxis { get; private set; }

    private int facingDirection = 1;
    private float walkTimer = 0f;
    private bool wasMovingLastFrame = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if ((x > 0 && transform.localScale.x < 0) ||
            (x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }

        bool isMoving = (x != 0 || y != 0);

        if (animator != null)
            animator.SetBool("IsMoving", isMoving);

        Vector2 v = new Vector2(x, y);
        MoveAxis = v.sqrMagnitude > 1f ? v.normalized : v;

        HandleWalkAudio(isMoving);
    }

    private void HandleWalkAudio(bool isMoving)
    {
        if (!isMoving)
        {
            walkTimer = 0f;
            wasMovingLastFrame = false;
            return;
        }

        if (!wasMovingLastFrame)
        {
            PlayWalkSound();
            walkTimer = 0f;
            wasMovingLastFrame = true;
            return;
        }

        walkTimer += Time.deltaTime;

        if (walkTimer >= walkInterval)
        {
            walkTimer = 0f;
            PlayWalkSound();
        }
    }

    private void PlayWalkSound()
    {
        if (audioSource == null || walkSound == null)
            return;

        audioSource.PlayOneShot(walkSound, walkVolume);
    }
}