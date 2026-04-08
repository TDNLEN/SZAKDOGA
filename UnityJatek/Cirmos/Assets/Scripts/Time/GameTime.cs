using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameTime : MonoBehaviour
{
    public static GameTime Instance;

    [Header("Time")]
    [Tooltip("Hány valódi másodperc teljen el 30 játékperchez")]
    public float realSecondsPerHalfHour = 10f;

    [Range(0, 23)] public int startHour = 12;
    [Range(0, 1)] public int startHalfIndex = 0; 

    [Header("UI")]
    public TMP_Text clockText;

    [Header("Lighting")]
    public UnityEngine.Rendering.Universal.Light2D globalLight;
    [Range(0f, 1f)] public float dayLightIntensity = 1f;
    [Range(0f, 1f)] public float nightLightIntensity = 0.45f;

    private float timer;
    private int currentHour;
    private int currentHalfIndex;

    private bool lastNightState;

    public int CurrentHour => currentHour;
    public bool IsNight => currentHour >= 21 || currentHour < 6;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHour = startHour;
        currentHalfIndex = startHalfIndex;

        UpdateClockUI();
        UpdateLightingImmediate();

        lastNightState = IsNight;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerHalfHour)
        {
            timer -= realSecondsPerHalfHour;
            AdvanceHalfHour();
        }
    }

    private void AdvanceHalfHour()
    {
        currentHalfIndex++;

        if (currentHalfIndex > 1)
        {
            currentHalfIndex = 0;
            currentHour++;

            if (currentHour >= 24)
                currentHour = 0;
        }

        UpdateClockUI();
        UpdateLightingImmediate();

        bool nowNight = IsNight;

        if (nowNight != lastNightState)
        {
            if (nowNight)
            {
                NightEvents.OnNightStarted?.Invoke();
            }
            else
            {
                NightEvents.OnDayStarted?.Invoke();
            }

            lastNightState = nowNight;
        }
    }

    private void UpdateClockUI()
    {
        string minutes = currentHalfIndex == 0 ? "00" : "30";

        if (clockText != null)
            clockText.text = $"{currentHour:00}:{minutes}";
    }

    private void UpdateLightingImmediate()
    {
        if (globalLight == null) return;

        globalLight.intensity = IsNight ? nightLightIntensity : dayLightIntensity;
    }
}