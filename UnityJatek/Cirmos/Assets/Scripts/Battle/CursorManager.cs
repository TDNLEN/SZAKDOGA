using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Textures")]
    public Texture2D defaultCursor;   // sima kurzor (vagy hagyd nullon → rendszer kurzor)
    public Texture2D crosshairCursor; // a piros célkereszt

    [Header("Hotspots (pixelben)")]
    public Vector2 defaultHotspot = Vector2.zero;
    public Vector2 crosshairHotspot = Vector2.zero;

    private static CursorManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        UseDefault();
    }

    public static void UseDefault()
    {
        if (instance == null)
            return;

        if (instance.defaultCursor == null)
        {
            // null → visszaáll a rendszer alap kurzorára
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(instance.defaultCursor, instance.defaultHotspot, CursorMode.Auto);
        }
    }

    public static void UseCrosshair()
    {
        if (instance == null || instance.crosshairCursor == null)
            return;

        Cursor.SetCursor(instance.crosshairCursor, instance.crosshairHotspot, CursorMode.Auto);
    }
}
