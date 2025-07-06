using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    private AudioManager audioManager;
    private Bus MusicBus;
    private Bus SFXBus;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager)
        {
            MusicBus = RuntimeManager.GetBus("bus:/Music");
            SFXBus = RuntimeManager.GetBus("bus:/SFX");
        }

        LoadVolume();
    }

    public void UpdateVolume()
    {
        float mVolume = musicSlider.value;
        float sVolume = sfxSlider.value;

        if (audioManager)
        {
            audioManager.MusicVolume = mVolume;
            audioManager.SFXVolume = sVolume;

            MusicBus.setVolume(mVolume / 100f);
            SFXBus.setVolume(sVolume / 100f);
        }
        if (audioMixer)
        {
            audioMixer.SetFloat("music", Mathf.Log10(mVolume) * 20);
            audioMixer.SetFloat("sfx", Mathf.Log10(sVolume) * 20);
        }

        PlayerPrefs.SetFloat("musicVolume", mVolume);
        PlayerPrefs.SetFloat("sfxVolume", sVolume);
    }

    private void LoadVolume()
    {
        if (musicSlider)
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        if (sfxSlider)
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        UpdateVolume();
    }
}
