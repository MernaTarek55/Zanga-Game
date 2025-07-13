using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public SoundLibrary soundLibrary;
    public AudioSource soundEffectSource;
    public AudioSource loopsoundEffectSource;
    public AudioSource musicSource;

    float musicVolume = .5f;
    float soundEffectVolume = .5f;

    public float MusicVolume => musicVolume;
    public float SoundEffectVolume => soundEffectVolume;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundType soundType)
    {
        AudioClip clip = soundLibrary.GetRandomClip(soundType);
        if (clip != null)
        {
            soundEffectSource.PlayOneShot(clip, soundEffectVolume);
        }

    }

    public void PlaySoundLoop(SoundType soundType)
    {
        AudioClip clip = soundLibrary.GetRandomClip(soundType);
        if (clip != null)
        {
            loopsoundEffectSource.clip = clip;
            loopsoundEffectSource.loop = true;
            loopsoundEffectSource.volume = soundEffectVolume;
            loopsoundEffectSource.Play();
        }

    }


    public void PlayMusic(SoundType soundType)
    {
        AudioClip clip = soundLibrary.GetRandomClip(soundType);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopSFX()
    {
        soundEffectSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        //PlayOneShot uses volume directly, so we don't change AudioSource.volume
        soundEffectVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", soundEffectVolume);
        PlayerPrefs.Save();
    }
    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        soundEffectVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }
    private void Start()
    {
        PlayMusic(SoundType.BG);
    }
}