using UnityEngine;

public class ObjectPulseEffect : MonoBehaviour
{
    public float speed = 2f;
    public float scaleAmount = 0.08f;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float scaleOffset = Mathf.Sin(Time.unscaledTime * speed) * scaleAmount;
        transform.localScale = startScale * (1f + scaleOffset);
    }
}