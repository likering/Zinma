using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // どこからでもアクセスできるシングルトン

    [Header("Audio Sources")]
    [SerializeField] private AudioSource seAudioSource; // SE再生用のAudioSource
    [SerializeField] private AudioSource bgmAudioSource; // BGM再生用のAudioSource

    [Header("UI効果音")]
    public AudioClip buttonClickSound;
    public AudioClip itemUsedSound;
    public AudioClip itemEquippedSound;

    // BGMのリストなどもここに追加できる
    // public AudioClip titleBGM;
    public AudioClip fieldBGM;
    void Awake()
    {
        // シングルトンの設定
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // 効果音を再生する汎用メソッド
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            seAudioSource.PlayOneShot(clip);
        }
    }

    // ★★★ ここからBGM用のメソッドを追加 ★★★

    /// <summary>
    /// 指定されたBGMを再生する
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource == null || clip == null) return;

        // もし再生しようとしている曲が、既に再生中の曲と同じなら、何もしない
        if (bgmAudioSource.clip == clip)
        {
            return;
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// BGMの再生を停止する
    /// </summary>
    public void StopBGM()
    {
        if (bgmAudioSource == null) return;
        bgmAudioSource.Stop();
    }

    public void CrossfadeBGM(AudioClip clip, float fadeDuration = 1.0f)
    {
        if (bgmAudioSource == null || clip == null || bgmAudioSource.clip == clip)
        {
            return; // オーディオソースがない、クリップがない、または同じ曲なら何もしない
        }

        // 既に実行中のフェード処理があれば停止し、新しいフェードを開始
        StopAllCoroutines();
        StartCoroutine(FadeMusic(clip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float duration)
    {
        float startVolume = bgmAudioSource.volume;

        // 現在の曲をフェードアウト
        while (bgmAudioSource.volume > 0)
        {
            bgmAudioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        // 新しい曲に差し替えて再生
        bgmAudioSource.Stop();
        bgmAudioSource.clip = newClip;
        bgmAudioSource.Play();

        // 新しい曲をフェードイン
        while (bgmAudioSource.volume < startVolume)
        {
            bgmAudioSource.volume += startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }
}