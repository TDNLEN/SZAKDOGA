using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [Header("References")]
    public Image[] slotBGs;      
    public Image[] slotIcons;      
    public RectTransform selector; 

    [Header("Colors")]
    public Color normalColor = new Color(1f, 1f, 1f, 0.2f);
    public Color selectedColor = new Color(1f, 1f, 1f, 0.5f);

    public int SelectedIndex { get; private set; } = 0;

    private void Start()
    {
        if (slotIcons != null && slotBGs != null && slotIcons.Length != slotBGs.Length)
        {
            Debug.LogWarning("[HotbarUI] slotIcons és slotBGs hossza eltér – ellenõrizd az Inspectorban!");
        }

        if (slotIcons != null)
        {
            for (int i = 0; i < slotIcons.Length; i++)
                SetIcon(i, null);
        }

        if (slotBGs != null && slotBGs.Length > 0)
            Select(0);
        else
            Debug.LogError("[HotbarUI] slotBGs nincs feltöltve az Inspectorban.");
    }

    public void SetIcon(int index, Sprite sprite)
    {
        if (slotIcons == null || index < 0 || index >= slotIcons.Length) return;

        var img = slotIcons[index];
        img.sprite = sprite;
        img.enabled = (sprite != null);
    }

    public void ClearIcon(int index) => SetIcon(index, (Sprite)null);

    public void Select(int index)
    {
        if (slotBGs == null || slotBGs.Length == 0)
        {
            Debug.LogError("[HotbarUI] slotBGs üres – kösd be a Slot képeket az Inspectorban!");
            return;
        }

        index = Mathf.Clamp(index, 0, slotBGs.Length - 1);
        SelectedIndex = index;

        if (selector)
        {
            selector.SetParent(slotBGs[SelectedIndex].transform, false);
            selector.anchoredPosition = Vector2.zero;
        }

        for (int i = 0; i < slotBGs.Length; i++)
            slotBGs[i].color = (i == SelectedIndex) ? selectedColor : normalColor;
    }

    public void SelectNext(int dir)
    {
        if (slotBGs == null || slotBGs.Length == 0) return;

        int i = (SelectedIndex + dir) % slotBGs.Length;
        if (i < 0) i += slotBGs.Length;
        Select(i);
    }
}
