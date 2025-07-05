using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    private AudioManager audioManager;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider SFXVolume;
    public Slider ambientVolume;

    // Audio Mixer
    private Bus MasterBus;
    private Bus AmbientBus;
    private Bus MusicBus;
    private Bus SFXBus;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        MasterBus = RuntimeManager.GetBus("bus:/");
        AmbientBus = RuntimeManager.GetBus("bus:/Ambient");
        MusicBus = RuntimeManager.GetBus("bus:/Music");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");

        UpdateVolume();
    }

    public void UpdateVolume()
    {
        if (audioManager)
        {
            audioManager.MasterVolume = masterVolume.value;
            audioManager.MusicVolume = musicVolume.value;
            audioManager.SFXVolume = SFXVolume.value;
            audioManager.AmbientVolume = ambientVolume.value;

            MasterBus.setVolume(audioManager.MasterVolume / 100f);
            AmbientBus.setVolume(audioManager.AmbientVolume / 100f);
            MusicBus.setVolume(audioManager.MusicVolume / 100f);
            SFXBus.setVolume(audioManager.SFXVolume / 100f);
        }
    }
}
