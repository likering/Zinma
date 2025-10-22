using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // どこからでもアクセスできるシングルトン

    private AudioSource audioSource;

    [Header("UI効果音")]
    public AudioClip buttonClickSound;
    public AudioClip itemUsedSound;
    public AudioClip itemEquippedSound;

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

        audioSource = GetComponent<AudioSource>();
    }

    // 効果音を再生する汎用メソッド
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}