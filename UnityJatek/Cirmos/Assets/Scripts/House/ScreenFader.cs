using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("UI")]
    public Image fadeImage;

    [Header("Timing")]
    public float defaultFadeOutTime = 0.6f;
    public float defaultFadeInTime = 0.6f;
    public float defaultBlackHoldTime = 0.15f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }
    }

    public void FadeOutIn(System.Action middleAction)
    {
        FadeOutIn(middleAction, defaultFadeOutTime, defaultBlackHoldTime, defaultFadeInTime);
    }

    public void FadeOutIn(System.Action middleAction, float fadeOutTime, float blackHoldTime, float fadeInTime)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeOutInRoutine(middleAction, fadeOutTime, blackHoldTime, fadeInTime));
    }

    private IEnumerator FadeOutInRoutine(System.Action middleAction, float fadeOutTime, float blackHoldTime, float fadeInTime)
    {
        if (fadeImage == null)
        {
            middleAction?.Invoke();
            yield break;
        }

        fadeImage.gameObject.SetActive(true);

        yield return StartCoroutine(FadeAlpha(0f, 1f, fadeOutTime));

        middleAction?.Invoke();

        if (blackHoldTime > 0f)
            yield return new WaitForSeconds(blackHoldTime);

        yield return StartCoroutine(FadeAlpha(1f, 0f, fadeInTime));

        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);

        currentRoutine = null;
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        Color c = fadeImage.color;

        if (duration <= 0f)
        {
            c.a = to;
            fadeImage.color = c;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(from, to, lerp);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}