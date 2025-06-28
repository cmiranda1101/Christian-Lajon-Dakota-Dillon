using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Music")]
    public AudioSource[] musicSources;

    public static float GlobalSFXVolume = 0.5f;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        foreach (AudioSource music in musicSources)
        {
            if (music != null)
                music.volume = value;
                music.ignoreListenerPause = true;
        }

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        GlobalSFXVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // This is the helper method  everywhere else
    public static void PlaySFX(AudioSource source, AudioClip clip)
    {
        if (source != null && clip != null)
            source.PlayOneShot(clip, GlobalSFXVolume);
    }
}







