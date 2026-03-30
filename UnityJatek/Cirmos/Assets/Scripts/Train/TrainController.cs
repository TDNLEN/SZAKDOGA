using UnityEngine;
using TMPro;

public class TrainController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform enterPoint;
    public Transform exitPoint;

    [Header("Visuals")]
    public SpriteRenderer[] trainSprites;
    public Color refuelFlashColor = new Color(1f, 0.6f, 0.2f);
    public float refuelFlashTime = 0.1f;

    private Color[] originalTrainColors;
    private Coroutine flashRoutine;

    [Header("Refuel")]
    public float refuelRadius = 6f;
    public PlayerInventory playerInventory;
    public GameObject iPrompt;

    [Header("Refuel Audio")]
    public AudioSource refuelAudioSource;
    public AudioClip refuelSound;
    [Range(0f, 1f)] public float refuelVolume = 1f;

    [Header("Train Move Audio")]
    public AudioSource moveAudioSource;
    public AudioClip trainMoveSound;
    [Range(0f, 1f)] public float trainMoveVolume = 1f;
    public float trainMoveInterval = 21f;

    private Coroutine trainMoveSoundRoutine;

    [Header("Wheels")]
    public Transform wheel;
    public float wheelMoveRadiusX = 0.1f;
    public float wheelMoveRadiusY = 0.05f;
    public float wheelAnimSpeed = 4f;

    private Vector3 wheelBaseLocalPos;
    private float wheelAnimTime = 0f;

    [Header("Enter settings")]
    public float enterRadius = 2f;
    public GameObject ePrompt;

    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Fuel")]
    public float maxFuel = 100f;
    public float fuelUsePerSecond = 1f;
    public TextMeshProUGUI fuelText;

    public bool IsRiding { get; private set; }
    public bool IsMoving { get; private set; }

    private Rigidbody2D playerRb;
    private TopDownMover mover;

    private float currentFuel;

    private void Awake()
    {
        currentFuel = maxFuel;
        UpdateFuelUI();

        if (wheel != null)
            wheelBaseLocalPos = wheel.localPosition;

        if (trainSprites != null && trainSprites.Length > 0)
        {
            originalTrainColors = new Color[trainSprites.Length];
            for (int i = 0; i < trainSprites.Length; i++)
            {
                if (trainSprites[i] != null)
                    originalTrainColors[i] = trainSprites[i].color;
            }
        }
    }

    private void Start()
    {
        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();

        if (fuelText != null)
            fuelText.gameObject.SetActive(false);

        if (ePrompt != null)
            ePrompt.SetActive(false);

        if (iPrompt != null)
            iPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!IsRiding)
            HandleEnter();
        else
            HandleRide();

        HandleRefuelPrompt();
        UpdateTrainMoveLoopState();
    }

    private void HandleEnter()
    {
        if (player == null || enterPoint == null) return;

        float dist = Vector2.Distance(player.position, enterPoint.position);
        bool canEnter = dist <= enterRadius;
        bool showPrompt = canEnter && !IsMoving;

        if (ePrompt != null)
            ePrompt.SetActive(showPrompt);

        if (canEnter && Input.GetKeyDown(KeyCode.E))
            EnterTrain();
    }

    private void EnterTrain()
    {
        IsRiding = true;
        IsMoving = false;

        playerRb = player.GetComponent<Rigidbody2D>();
        mover = player.GetComponent<TopDownMover>();
        if (mover) mover.enabled = false;

        player.position = enterPoint.position;
        if (playerRb) playerRb.linearVelocity = Vector2.zero;

        if (ePrompt != null)
            ePrompt.SetActive(false);

        UpdateFuelUI();
    }

    private void ExitTrain()
    {
        IsRiding = false;
        IsMoving = false;

        StopTrainMoveLoopImmediately();

        if (mover) mover.enabled = true;

        if (exitPoint != null)
            player.position = exitPoint.position;

        UpdateFuelUI();

        if (ePrompt != null)
            ePrompt.SetActive(false);
    }

    private void HandleRide()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitTrain();
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");
        bool canMove = currentFuel > 0.01f;
        IsMoving = canMove && Mathf.Abs(input) > 0.01f;

        if (IsMoving)
        {
            Vector3 delta = new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime;
            transform.position += delta;

            ConsumeFuel(Time.deltaTime);
        }

        player.position = enterPoint.position;

        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        if (wheel != null)
        {
            if (IsMoving)
            {
                wheelAnimTime += Time.deltaTime * wheelAnimSpeed;

                float offsetX = Mathf.Cos(wheelAnimTime) * wheelMoveRadiusX;
                float offsetY = Mathf.Sin(wheelAnimTime) * wheelMoveRadiusY;

                wheel.localPosition = wheelBaseLocalPos + new Vector3(offsetX, offsetY, 0f);
            }
            else
            {
                wheel.localPosition = wheelBaseLocalPos;
                wheelAnimTime = 0f;
            }
        }
    }

    private void UpdateTrainMoveLoopState()
    {
        if (IsMoving)
        {
            if (trainMoveSoundRoutine == null)
                trainMoveSoundRoutine = StartCoroutine(TrainMoveLoopRoutine());
        }
        else
        {
            StopTrainMoveLoopImmediately();
        }
    }

    private System.Collections.IEnumerator TrainMoveLoopRoutine()
    {
        PlayTrainMoveSound();

        while (true)
        {
            yield return new WaitForSeconds(trainMoveInterval);

            if (!IsMoving)
                break;

            PlayTrainMoveSound();
        }

        trainMoveSoundRoutine = null;
    }

    private void PlayTrainMoveSound()
    {
        if (moveAudioSource == null || trainMoveSound == null)
            return;

        moveAudioSource.PlayOneShot(trainMoveSound, trainMoveVolume);
    }

    private void StopTrainMoveLoopImmediately()
    {
        if (trainMoveSoundRoutine != null)
        {
            StopCoroutine(trainMoveSoundRoutine);
            trainMoveSoundRoutine = null;
        }

        if (moveAudioSource != null && moveAudioSource.isPlaying)
            moveAudioSource.Stop();
    }

    private void HandleRefuelPrompt()
    {
        if (player == null || playerInventory == null || iPrompt == null)
            return;

        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= refuelRadius;
        bool hasFuelItem = playerInventory.HasFuelInSelectedSlot();
        bool notFull = currentFuel < maxFuel - 0.01f;

        bool showI = inRange && hasFuelItem && notFull;

        iPrompt.SetActive(showI);

        if (showI && Input.GetKeyDown(KeyCode.I))
        {
            TryRefuel();
        }
    }

    private void TryRefuel()
    {
        if (playerInventory == null) return;

        if (playerInventory.TryConsumeSelectedFuelItem(out int fuelGained))
        {
            float oldFuel = currentFuel;
            currentFuel = Mathf.Clamp(currentFuel + fuelGained, 0f, maxFuel);
            UpdateFuelUI();

            Debug.Log($"Train refueled: +{fuelGained} fuel → {currentFuel}/{maxFuel}");

            if (currentFuel > oldFuel)
                PlayRefuelSound();

            if (trainSprites != null && trainSprites.Length > 0)
            {
                if (flashRoutine != null)
                    StopCoroutine(flashRoutine);

                flashRoutine = StartCoroutine(RefuelFlash());
            }
        }
        else
        {
            Debug.Log("Nincs égethető item a kiválasztott slotban.");
        }
    }

    private void PlayRefuelSound()
    {
        if (refuelAudioSource == null || refuelSound == null)
            return;

        refuelAudioSource.PlayOneShot(refuelSound, refuelVolume);
    }

    private void ConsumeFuel(float deltaTime)
    {
        currentFuel -= fuelUsePerSecond * deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        UpdateFuelUI();
    }

    private void UpdateFuelUI()
    {
        if (fuelText == null) return;

        if (!IsRiding)
        {
            fuelText.gameObject.SetActive(false);
            return;
        }

        fuelText.gameObject.SetActive(true);

        int cur = Mathf.RoundToInt(currentFuel);
        int max = Mathf.RoundToInt(maxFuel);
        fuelText.text = $"Fuel: {cur}/{max}";
    }

    private System.Collections.IEnumerator RefuelFlash()
    {
        if (trainSprites == null || trainSprites.Length == 0) yield break;

        for (int i = 0; i < trainSprites.Length; i++)
        {
            if (trainSprites[i] != null)
                trainSprites[i].color = refuelFlashColor;
        }

        yield return new WaitForSeconds(refuelFlashTime);

        for (int i = 0; i < trainSprites.Length; i++)
        {
            if (trainSprites[i] != null)
                trainSprites[i].color = originalTrainColors[i];
        }

        flashRoutine = null;
    }
}