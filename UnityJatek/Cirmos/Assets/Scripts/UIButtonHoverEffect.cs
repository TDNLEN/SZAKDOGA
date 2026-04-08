using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale settings")]
    public float normalScale = 1f;
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSound;   
    public AudioClip clickSound;  

    private RectTransform rectTransform;
    private Vector3 targetScale;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetScale = Vector3.one * normalScale;
        rectTransform.localScale = targetScale;
    }

    void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * scaleSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = Vector3.one * hoverScale;

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector3.one * normalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}