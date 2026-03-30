using UnityEngine;

[DisallowMultipleComponent]
public class MeleeWeaponAudio : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    [Range(0f, 1f)] public float attackVolume = 1f;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound()
    {
        if (audioSource == null || attackSound == null)
            return;

        audioSource.PlayOneShot(attackSound, attackVolume);
    }
}