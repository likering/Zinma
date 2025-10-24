using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("このシーンで再生したいBGM")]
    public AudioClip fieldBGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AudioManagerが存在するか確認
        if (AudioManager.instance != null)
        {
            // AudioManagerに、このシーンのBGMを再生するように依頼する
            AudioManager.instance.PlayBGM(fieldBGM);
        }
        else
        {
            Debug.LogError("AudioManager.instance が見つかりません！シーンにAudioManagerが存在するか確認してください。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
