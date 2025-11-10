using UnityEngine;
using TMPro;

public class TrainController : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform enterPoint;
    public Transform exitPoint;

    [Header("Refuel")]
    public float refuelRadius = 6f;           // 6 egységen belül lehet tankolni
    public PlayerInventory playerInventory;   // ide húzd a PlayerInventory-t
    public GameObject iPrompt;                // I gomb ikon a player fején


    [Header("Enter settings")]
    public float enterRadius = 2f;
    public GameObject ePrompt;

    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Fuel")]
    public float maxFuel = 100f;
    public float fuelUsePerSecond = 1f;      // ennyit fogyaszt másodpercenként mozgás közben
    public TextMeshProUGUI fuelText;         // ide húzd be a TrainFuelText-et

    public bool IsRiding { get; private set; }
    public bool IsMoving { get; private set; }

    private Rigidbody2D playerRb;
    private TopDownMover mover;

    private float currentFuel;

    private void Awake()
    {
        currentFuel = maxFuel;
        UpdateFuelUI();
    }

    private void Start()
    {
        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();

        // induláskor fuel UI legyen kikapcsolva
        if (fuelText != null)
            fuelText.gameObject.SetActive(false);

        // E-prompt se világítson alapból
        if (ePrompt != null)
            ePrompt.SetActive(false);

        if (iPrompt != null)
            iPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!IsRiding)
        {
            HandleEnter();
            HandleRefuelPrompt();   // <-- kívül állva is lehessen tankolni, ha közel vagy
        }
        else
        {
            HandleRide();
            HandleRefuelPrompt();   // <-- a vonatban ülve is mehet tankolás + I_prompt
        }
    }


    // --- beszállás logika ---
    private void HandleEnter()
    {
        if (player == null || enterPoint == null) return;

        float dist = Vector2.Distance(player.position, enterPoint.position);
        bool canEnter = dist <= enterRadius;

        // ha a vonat "menne" (később esetleg), akkor se jelezzük
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

        // első “ragasztás”
        player.position = enterPoint.position;
        if (playerRb) playerRb.linearVelocity = Vector2.zero;

        // E-prompt el
        if (ePrompt != null)
            ePrompt.SetActive(false);

        // fuel UI bekapcs, érték frissítés
        UpdateFuelUI();
    }

    private void ExitTrain()
    {
        IsRiding = false;
        IsMoving = false;

        if (mover) mover.enabled = true;

        // ha akarod, kicsit a vonat mellé rakhatod
        if (exitPoint != null)
            player.position = exitPoint.position;

        // fuel UI most TUTI el fog tűnni
        UpdateFuelUI();

        // E-prompt majd csak akkor jelenik meg, ha újra közel mész
        if (ePrompt != null)
            ePrompt.SetActive(false);
    }

    // --- vonaton ülés + mozgás ---
    private void HandleRide()
    {
        if (player == null) return;

        // E – kiszállás
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitTrain();
            return;
        }

      

        float input = Input.GetAxisRaw("Horizontal");

        // csak akkor mozoghat a vonat, ha VAN fuel
        bool canMove = currentFuel > 0.01f;
        IsMoving = canMove && Mathf.Abs(input) > 0.01f;

        if (IsMoving)
        {
            Vector3 delta = new Vector3(input, 0f, 0f) * moveSpeed * Time.deltaTime;
            transform.position += delta;

            // fuel fogyasztás
            ConsumeFuel(Time.deltaTime);
        }

        // akár mozog a vonat, akár nem, amíg a vonaton ülünk,
        // a player pozíciója mindig az EnterPoint legyen.
        player.position = enterPoint.position;

        // ha van Rigidbody2D, lenullázzuk a sebességét,
        // hogy semmi ne tudja “kilökni” a vonatból
        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;
    }


    // --- ÜZEMANYAG ---
    private void HandleRefuelPrompt()
    {
        if (player == null || playerInventory == null || iPrompt == null)
            return;

        // vonat–player távolság
        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= refuelRadius;

        // van-e éghető item a kiválasztott slotban?
        bool hasFuelItem = playerInventory.HasFuelInSelectedSlot();

        // csak akkor engedjük a tankolást, ha nincs tele
        bool notFull = currentFuel < maxFuel - 0.01f;

        // csak akkor mutassuk az I ikont, ha mindhárom feltétel igaz
        bool showI = inRange && hasFuelItem && notFull;

        iPrompt.SetActive(showI);

        // ha látszik az I és lenyomja az I-t → tankolás
        if (showI && Input.GetKeyDown(KeyCode.I))
        {
            TryRefuel();
        }
        // ha tele van → mindig tűnjön el az ikon (biztonsági)
        else if (!notFull && iPrompt.activeSelf)
        {
            iPrompt.SetActive(false);
        }
    }


    private void TryRefuel()
    {
        if (playerInventory == null) return;

        if (currentFuel >= maxFuel)
            return; // tele van, fölösleges

        if (playerInventory.TryConsumeSelectedFuelItem(out int fuelGained))
        {
            currentFuel = Mathf.Clamp(currentFuel + fuelGained, 0f, maxFuel);
            UpdateFuelUI();
            Debug.Log($"Train refueled: +{fuelGained} fuel → {currentFuel}/{maxFuel}");
        }
        else
        {
            Debug.Log("Nincs égethető item a kiválasztott slotban.");
        }
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

        // csak akkor mutatjuk, ha a vonaton ülünk
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
}
