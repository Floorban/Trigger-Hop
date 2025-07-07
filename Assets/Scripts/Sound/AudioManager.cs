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
}
