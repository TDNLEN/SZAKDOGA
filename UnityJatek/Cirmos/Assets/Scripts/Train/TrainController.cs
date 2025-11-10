using UnityEngine;

public class TrainController : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // Player
    public Transform enterPoint;   // ülés helye a vonaton
    public Transform exitPoint;    // kiszállási pont
    public GameObject ePrompt;     // lebegõ "E" ikon

    [Header("Settings")]
    public float enterDistance = 2f;
    public float moveSpeed = 5f;

    private bool isRiding = false;

    // player komponensek
    TopDownMover playerMover;
    PlayerInputReader playerInput;
    Rigidbody2D playerRb;
    Collider2D playerCol;
    SpriteRenderer playerSprite;

    // sprite eredeti layer / order
    string origLayer;
    int origOrder;

    // melyik local poziban üljön (EnterPoint-hoz mérten)
    Vector3 rideLocalPos;


    public bool IsRiding { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;   // <<< EZ ÚJ

    void Start()
    {
        if (player != null)
        {
            playerMover = player.GetComponent<TopDownMover>();
            playerInput = player.GetComponent<PlayerInputReader>();
            playerRb = player.GetComponent<Rigidbody2D>();
            playerCol = player.GetComponent<Collider2D>();
            playerSprite = player.GetComponent<SpriteRenderer>();

            if (playerSprite != null)
            {
                origLayer = playerSprite.sortingLayerName;
                origOrder = playerSprite.sortingOrder;
            }
        }

        if (ePrompt != null)
            ePrompt.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        if (!isRiding)
            HandleEnter();
        else
            HandleRide();
    }

    // --- beszállás logika ---
    void HandleEnter()
    {
        if (enterPoint == null) return;

        float dist = Vector2.Distance(player.position, enterPoint.position);
        bool canEnter = dist <= enterDistance;

        if (ePrompt != null)
            ePrompt.SetActive(canEnter);

        if (canEnter && Input.GetKeyDown(KeyCode.E))
            EnterTrain();
        IsMoving = false;

    }

    // --- vonaton ülés közben ---
    void HandleRide()
    {
        if (ePrompt != null)
            ePrompt.SetActive(false);

        float h = Input.GetAxisRaw("Horizontal");
        Vector3 delta = new Vector3(h * moveSpeed * Time.deltaTime, 0f, 0f);

        // CSAK a vonat mozog
        transform.position += delta;

        // player mindig fixen az ülésen
        if (player != null)
            player.localPosition = rideLocalPos;

        // itt jelezzük, hogy épp mozog-e a vonat
        IsMoving = Mathf.Abs(h) > 0.01f;

        if (Input.GetKeyDown(KeyCode.E))
            ExitTrain();
    }


    void EnterTrain()
    {
        isRiding = true;

        // mozgás letiltása
        if (playerMover) playerMover.enabled = false;
        if (playerInput) playerInput.enabled = false;

        // fizika off, hogy semmi ne lökje el
        if (playerRb)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.simulated = false;
        }
        if (playerCol) playerCol.enabled = false;

        // sprite a vonat mögé
        if (playerSprite != null)
        {
            playerSprite.sortingLayerName = "Background"; // ugyanaz, mint a train_up_0
            playerSprite.sortingOrder = -1;
        }

        // parent = Train + fix pozíció
        player.SetParent(transform);

        if (enterPoint != null)
        {
            // kiszámoljuk, hogy az enterPoint a vonat local terében hol van
            rideLocalPos = transform.InverseTransformPoint(enterPoint.position);
            player.localPosition = rideLocalPos;
        }
        else
        {
            rideLocalPos = player.localPosition;
        }
    }

    void ExitTrain()
    {
        isRiding = false;

        // leválasztjuk a vonatról
        player.SetParent(null);

        // sprite vissza
        if (playerSprite != null)
        {
            playerSprite.sortingLayerName = origLayer;
            playerSprite.sortingOrder = origOrder;
        }

        // fizika vissza
        if (playerRb) playerRb.simulated = true;
        if (playerCol) playerCol.enabled = true;

        // mozgás engedélyezése
        if (playerMover) playerMover.enabled = true;
        if (playerInput) playerInput.enabled = true;

        // kiléptetés helye
        if (exitPoint != null)
            player.position = exitPoint.position;

        IsRiding = false;
        IsMoving = false;
    }
}
