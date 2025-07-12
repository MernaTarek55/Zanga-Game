using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsSlider : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundEffectSlider;

    private void Start()
    {
        musicSlider.value = AudioManager.Instance.MusicVolume;
        soundEffectSlider.value = AudioManager.Instance.SoundEffectVolume;

        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        soundEffectSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
    }

    private void OnSFXVolumeChanged(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }
}
