using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;
    private AudioSource audioSource;
    private SoundEffectLibrary soundEffectLibrary;
    [SerializeField] private Slider sfxSlider;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Play(string soundName)
    {
       AudioClip audioClip = SoundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
           audioSource.PlayOneShot(audioClip, Instance.sfxSlider.value);
        }
    }   

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public static void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }

    public void Start()
    {
        
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }
}
