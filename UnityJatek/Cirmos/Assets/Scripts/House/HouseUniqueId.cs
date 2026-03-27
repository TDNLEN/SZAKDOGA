using UnityEngine;

public class HouseUniqueId : MonoBehaviour
{
    public string uniqueId;

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(uniqueId) || uniqueId.StartsWith("house_manual_"))
        {
            int px = Mathf.RoundToInt(transform.position.x * 100f);
            int py = Mathf.RoundToInt(transform.position.y * 100f);
            uniqueId = $"house_{px}_{py}";
        }
    }
}