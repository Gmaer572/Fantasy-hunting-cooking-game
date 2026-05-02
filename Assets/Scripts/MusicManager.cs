using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MusicLibrary))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;
    private MusicLibrary musicLibrary;
    [SerializeField] private Slider musicSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        musicLibrary = GetComponent<MusicLibrary>();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        musicSlider?.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
    }

    public static void Play(string trackName)
    {
        if (Instance == null) { Debug.LogWarning("MusicManager: no instance in scene."); return; }

        AudioClip clip = Instance.musicLibrary.GetClip(trackName);
        if (clip == null) { Debug.LogWarning($"MusicManager: no track found for \"{trackName}\"."); return; }

        if (Instance.audioSource.clip == clip) return;

        Instance.audioSource.clip = clip;
        Instance.audioSource.Play();
    }

    public static void SetVolume(float volume)
    {
        if (Instance != null)
            Instance.audioSource.volume = volume * 0.5f;
    }

    public static void Pause()  => Instance?.audioSource.Pause();
    public static void Resume() => Instance?.audioSource.UnPause();
    public static void Stop()   => Instance?.audioSource.Stop();
}
