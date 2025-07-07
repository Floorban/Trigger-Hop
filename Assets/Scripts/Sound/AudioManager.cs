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
                     spin,
                     bullet1,
                     bullet2,
                     bullet3,
                     firePistol,
                     fireShotgun,
                     fireRifle,
                     reloadPistol,
                     reloadShotgun,
                     reloadRifle;
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
