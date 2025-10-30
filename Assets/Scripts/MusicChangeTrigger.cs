
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [Header("このエリアで再生するBGM")]
    public AudioClip areaBGM;

    [Header("オプション")]
    [SerializeField] private bool useCrossfade = true;
    [SerializeField] private float fadeDuration = 1.0f;

    // 一度だけ実行するためのフラグ
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーが侵入し、まだトリガーが作動していない場合
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true; // 作動済みにする

            if (AudioManager.instance != null)
            {
                if (useCrossfade)
                {
                    AudioManager.instance.CrossfadeBGM(areaBGM, fadeDuration);
                }
                else
                {
                    AudioManager.instance.PlayBGM(areaBGM);
                }
            }
        }
    }
}
