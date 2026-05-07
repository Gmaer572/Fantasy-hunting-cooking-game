using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;
    private AudioSource audioSource;
    private SoundEffectLibrary soundEffectLibrary;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private float sfxVolumeMultiplier = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        soundEffectLibrary = GetComponent<SoundEffectLibrary>();
        if (SceneManager.GetActiveScene().name != "Dialogue") DontDestroyOnLoad(gameObject);
        // Warm up the audio pipeline to eliminate first-play latency.
        audioSource.PlayOneShot(AudioClip.Create("warmup", 1, 1, 44100, false), 0f);
    }

    private void Start()
    {
        sfxSlider?.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public static void Play(string soundName, float volumeScale = 1f)
    {
        if (Instance == null) { Debug.LogWarning("SoundEffectManager: no instance in scene."); return; }
        if (Instance.soundEffectLibrary == null) { Debug.LogWarning("SoundEffectManager: SoundEffectLibrary component missing."); return; }
        if (Instance.audioSource == null) { Debug.LogWarning("SoundEffectManager: AudioSource missing."); return; }

        AudioClip audioClip = Instance.soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip == null) { Debug.LogWarning($"SoundEffectManager: no clip found for \"{soundName}\"."); return; }

        float volume = (Instance.sfxSlider != null ? Instance.sfxSlider.value : 1f) * Instance.sfxVolumeMultiplier * volumeScale;
        Instance.audioSource.PlayOneShot(audioClip, volume);
    }

    public static void OnValueChanged()
    {
        // Volume is applied per-clip in PlayOneShot — nothing extra needed here.
    }
}
