using UnityEngine;

public class EnemyProximitySound : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public AudioSource audioSource;

    [Header("Sound")]
    public AudioClip proximitySound;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Distance")]
    public float triggerDistance = 10f;

    [Header("Timing")]
    public float repeatInterval = 9f;

    private float timer = 0f;
    private bool wasInRangeLastFrame = false;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (player == null || audioSource == null || proximitySound == null)
            return;

        float dist = Vector2.Distance(player.position, transform.position);
        bool inRange = dist <= triggerDistance;

        if (!inRange)
        {
            timer = 0f;
            wasInRangeLastFrame = false;
            return;
        }

        // Belépéskor azonnal lejátssza
        if (!wasInRangeLastFrame)
        {
            PlaySound();
            timer = 0f;
            wasInRangeLastFrame = true;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= repeatInterval)
        {
            timer = 0f;
            PlaySound();
        }
    }

    private void PlaySound()
    {
        audioSource.PlayOneShot(proximitySound, volume);
    }
}