using UnityEngine;

public class HouseEntrance : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform interactPoint;
    public GameObject ePrompt;

    [Header("Dungeon")]
    public DungeonConfig dungeonConfig;

    [Header("Settings")]
    public float interactRadius = 3f;
    public KeyCode interactKey = KeyCode.E;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (interactPoint == null)
            interactPoint = transform;
    }

    private void Start()
    {
        if (ePrompt != null)
            ePrompt.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(player.position, interactPoint.position);
        bool inRange = dist <= interactRadius;

        if (ePrompt != null)
            ePrompt.SetActive(inRange);

        if (inRange && Input.GetKeyDown(interactKey))
        {
            DungeonManager mgr = FindFirstObjectByType<DungeonManager>();
            if (mgr != null)
                mgr.EnterDungeonFromHouse(this, dungeonConfig);
        }
    }
}