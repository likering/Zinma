using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutineのために追加

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("エンディング設定")]
    [SerializeField] private string endingScenesName = "EndingScenes"; // 遷移するエンディングシーン名
    [SerializeField] private float timeToLoadEnding = 5.0f; // アイテム入手メッセージ表示後、シーン遷移までの待機時間

    private bool isEndingTriggered = false; // エンディングがトリガーされたか

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移しても破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は自身を破棄
        }
    }

    // プレイヤーがエンディングアイテムを拾ったときに呼ばれる
    public void PlayerCollectedEndingItem()
    {
        if (isEndingTriggered) return; // 既にエンディングがトリガーされている場合は何もしない

        isEndingTriggered = true;
        Debug.Log("エンディング条件達成！プレイヤーがエンディングアイテムを拾いました。");
        // （任意）ここでクリアメッセージを表示するなどの演出を追加できる

        StartCoroutine(LoadEndingSceneAfterDelay());
    }

    private IEnumerator LoadEndingSceneAfterDelay()
    {
        yield return new WaitForSeconds(timeToLoadEnding);
        SceneManager.LoadScene(endingScenesName);
    }
}