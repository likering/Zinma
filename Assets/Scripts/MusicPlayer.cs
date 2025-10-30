using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("このシーンで再生したいBGM")]
    public AudioClip sceneBGM;

    [Header("オプション")]
    [SerializeField] private bool useCrossfade = true;
    [SerializeField] private float fadeDuration = 1.5f;

    void Start()
    {
        if (AudioManager.instance != null)
        {
            Debug.Log(this.gameObject.scene.name + " シーンが開始しました。BGM: " + sceneBGM.name + " を再生します。");
            if (useCrossfade)
            {
                AudioManager.instance.CrossfadeBGM(sceneBGM, fadeDuration);
            }
            else
            {
                AudioManager.instance.PlayBGM(sceneBGM);
            }
        }
        else
        {
            Debug.LogError("AudioManager.instance が見つかりません！");
            Debug.LogWarning(this.gameObject.name + " の MusicPlayerにBGMが設定されていません！");
        }
    }
}
