using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Unity Audio")]
    public AudioSource musicSource, sfxSource;

    [Header("Audio Clip")]
    public AudioClip bgm,
                      btnSelect,
                      btnConfirm,
                      gameStart,
                      gameOver,
                      coinCollect,
                      lvlFinished;
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
