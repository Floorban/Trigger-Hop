using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private void Awake() => LoadVolume();

    public void UpdateVolume()
    {
        float mVolume = musicSlider.value;
        float sVolume = sfxSlider.value;

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
        UpdateVolume();
        if (musicSlider)
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        if (sfxSlider)
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    }
}
