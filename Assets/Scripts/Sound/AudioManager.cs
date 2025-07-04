using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [Header("Unity Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip bgm,
                      btnSelect,
                      btnConfirm,
                      gameStart,
                      gameOver,
                      coinCollect,
                      lvlFinished;
    public void PlaySfx(AudioClip clip) => sfxSource.PlayOneShot(clip);

    [Header("FMOD")]
    [Tooltip("The value that controls the volume for the master mixer, which contains all the sound in the game. You can only change the volume before playing.")]
    [Range(0, 100)] public float MasterVolume = 50;

    [Tooltip("The value which controls ambient volume specifically. You can only change the volume before playing.")]
    [Range(0, 100)] public float AmbientVolume = 100;

    [Tooltip("The value which controls music volume specifically. You can only change the volume before playing.")]
    [Range(0, 100)] public float MusicVolume = 100;

    [Tooltip("The value which controls sound effects volume specifically. You can only change the volume before playing.")]
    [Range(0, 100)] public float SFXVolume = 100;

    [Header("FMOD Parameters")]
    [Tooltip("The intensity of the randomness of the sound, 0% is no randomness, 100% is full randomness, this can be used to add variety to sounds, such as stylization, pitch, volume etc.")]
    [SerializeField][Range(0, 100)] private int GlobalRandomnessIntensityValue = 50;

    [Tooltip("A list of custom parameters that can be used to change the intensities for sounds, add your own custom parameter(s) here that you would like to be able to change")]
    public List<string> CustomParameter = new List<string>();

    [Header("Developer Attributes")]
    [SerializeField] bool developerMode = false;
    [SerializeField] EventReference SFX_TestAudio;

    [SerializeField] ProceduralGun proc;

    public enum GameState { MainMenu, Play, Pause }
    public GameState gameState = GameState.Play;

    private string RandomnessIntensity = "RandomnessIntensity";

    private Bus MasterBus;
    private Bus AmbientBus;
    private Bus MusicBus;
    private Bus SFXBus;

    private void Update()
    {
        if (developerMode) if (Input.GetKeyDown(KeyCode.P)) proc.PlaySound(transform.position);
        if (developerMode) if (Input.GetKeyDown(KeyCode.R)) proc.RandomizeSound(transform.position);
    }

    public void PlaySound( // Plays a oneshot SFX with position-dependent audio, where parameters cannot be changed after initiation, ideal for short-duration SFX
        EventReference sfx,
        Vector3 playPosition,

        string customParameter = null,
        float customParameterValue = 0f,

        string customParameter2 = null,
        float customParameter2Value = 0f
    )
    {
        if (!sfx.IsNull)
        {
            EventInstance sfxInstance = CreateInstance(sfx, playPosition);
            sfxInstance = CreateInstance(sfx, playPosition);

            sfxInstance.setParameterByName(customParameter, customParameterValue);
            sfxInstance.setParameterByName(customParameter2, customParameter2Value);

            sfxInstance.start();
            sfxInstance.release();
        }
        else Debug.Log("Sound not found: " + sfx);

    }

    public EventInstance PlayInstance( // Plays a looping sound with position-dependent audio, where parameters can be changed after initiation, ideal for long-duration sounds
        EventReference sound,
        Vector3 playPosition,
        GameObject gameObject,
        int randomnessIntensityValue = -1
    )
    {
        if (!sound.IsNull)
        {
            EventInstance loopInstance = CreateInstance(sound, playPosition);
            RuntimeManager.AttachInstanceToGameObject(loopInstance, gameObject);
            if (randomnessIntensityValue != 0) loopInstance.setParameterByName(RandomnessIntensity, randomnessIntensityValue);
            if (randomnessIntensityValue < 0) loopInstance.setParameterByName(RandomnessIntensity, GlobalRandomnessIntensityValue);
            loopInstance.start();
            return loopInstance;
        }
        else Debug.Log("Sound not found: " + sound);
        return default(EventInstance);
    }

    public EventInstance CreateInstance(EventReference audio, Vector3 eventPosition)
    {
        EventInstance audioInstance = RuntimeManager.CreateInstance(audio);
        audioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(eventPosition));
        return audioInstance;
    }

    public void ContinueInstance(params EventInstance[] audioInstances)
    {
        foreach (var instance in audioInstances)
        {
            instance.setPaused(false);
        }
    }
    public void PauseInstance(params EventInstance[] audioInstances)
    {
        foreach (var instance in audioInstances)
        {
            instance.setPaused(true);
        }
    }

    public void StopInstance(params EventInstance[] audioInstances)
    {
        foreach (var instance in audioInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void ClearInstance(params EventInstance[] audioInstances)
    {
        foreach (EventInstance instance in audioInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    public bool IsAudioPlaying(params EventInstance[] audioInstances)
    {
        foreach (EventInstance instance in audioInstances)
        {
            instance.getPaused(out bool paused);
            if (paused) return false;
        }
        return true;
    }

    public void SetParameter(EventInstance audioInstance, string parameterName, float value) => audioInstance.setParameterByName(parameterName, value);

    void RefreshMixer()
    {
        AmbientBus.setMute(false); AmbientBus.setPaused(false);
        MusicBus.setMute(false); MusicBus.setPaused(false);
        SFXBus.setMute(false); SFXBus.setPaused(false);
    }

    public void AdjustAudioToState(GameState state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.Play:
                RefreshMixer();
                break;
            case GameState.Pause:
                RefreshMixer();
                SFXBus.setPaused(true);
                AmbientBus.setPaused(true);
                break;
            case GameState.MainMenu:
                RefreshMixer();
                break;
        }
    }
}
