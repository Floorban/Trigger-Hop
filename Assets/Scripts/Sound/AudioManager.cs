using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgm;
    public AudioClip btnSelect,
                     btnConfirm,
                     gameStart,
                     gameOver,
                     coinCollect,
                     lvlFinished,
                     spin;

    private void Awake()
    {
        if (bgm)
        {
            musicSource.clip = bgm;
            musicSource.Play();
        }
    }
    public void PlaySfx(AudioClip clip) => sfxSource.PlayOneShot(clip);
}
