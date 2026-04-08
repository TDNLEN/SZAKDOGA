using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public enum MusicType
    {
        None,
        Menu,
        Day,
        Night,
        Dungeon
    }

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip menuMusic;
    public AudioClip dayMusic;
    public AudioClip nightMusic;
    public AudioClip dungeonMusic;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    private MusicType currentMusic = MusicType.None;
    private bool isInDungeon = false;
    private bool isNight = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        NightEvents.OnNightStarted += HandleNightStarted;
        NightEvents.OnDayStarted += HandleDayStarted;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            NightEvents.OnNightStarted -= HandleNightStarted;
            NightEvents.OnDayStarted -= HandleDayStarted;
        }
    }

    private void Start()
    {
        RefreshMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshMusic();
    }

    private void HandleNightStarted()
    {
        isNight = true;
        RefreshMusic();
    }

    private void HandleDayStarted()
    {
        isNight = false;
        RefreshMusic();
    }

    public void SetDungeonState(bool value)
    {
        isInDungeon = value;
        RefreshMusic();
    }

    public void RefreshMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == mainMenuSceneName)
        {
            PlayMusic(MusicType.Menu);
            return;
        }

        if (isInDungeon)
        {
            PlayMusic(MusicType.Dungeon);
            return;
        }

        if (GameTime.Instance != null)
            isNight = GameTime.Instance.IsNight;

        if (isNight)
            PlayMusic(MusicType.Night);
        else
            PlayMusic(MusicType.Day);
    }

    private void PlayMusic(MusicType type)
    {
        if (musicSource == null) return;
        if (currentMusic == type && musicSource.isPlaying) return;

        AudioClip targetClip = GetClip(type);
        if (targetClip == null) return;

        currentMusic = type;
        musicSource.clip = targetClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private AudioClip GetClip(MusicType type)
    {
        switch (type)
        {
            case MusicType.Menu: return menuMusic;
            case MusicType.Day: return dayMusic;
            case MusicType.Night: return nightMusic;
            case MusicType.Dungeon: return dungeonMusic;
            default: return null;
        }
    }
}