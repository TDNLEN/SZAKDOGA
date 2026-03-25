using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerNightLight : MonoBehaviour
{
    public Light2D playerLight;
    public float dayIntensity = 0f;
    public float nightIntensity = 1f;

    private void Update()
    {
        if (playerLight == null || GameTime.Instance == null) return;

        playerLight.intensity = GameTime.Instance.IsNight ? nightIntensity : dayIntensity;
    }
}