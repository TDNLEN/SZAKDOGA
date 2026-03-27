using UnityEngine;

public class DungeonExitInteractor : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public Transform interactPoint;
    public GameObject ePrompt;

    [Header("Settings")]
    public float interactRadius = 3f;
    public KeyCode interactKey = KeyCode.E;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
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
        if (interactPoint == null) return;

        float dist = Vector2.Distance(player.position, interactPoint.position);
        bool inRange = dist <= interactRadius;

        if (ePrompt != null)
            ePrompt.SetActive(inRange);

        if (inRange && Input.GetKeyDown(interactKey))
        {
            DungeonManager mgr = FindFirstObjectByType<DungeonManager>();
            if (mgr != null)
                mgr.ExitDungeon();
        }
    }
}