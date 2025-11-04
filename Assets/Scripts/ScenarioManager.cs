using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshProを扱うために必要

public class ScenarioManager : MonoBehaviour
{
    // Unityのインスペクターから設定する項目
    [SerializeField] private TextMeshProUGUI scenarioText; // セリフを表示するUIテキスト
    [SerializeField] private string nextSceneName = "SampleScene"; // 次に遷移するシーン名（あなたのメインシーン名に書き換えてください）

    // 表示するシナリオの全文をここに記述
    [SerializeField, TextArea(3, 10)]
    private string[] scenarios = {
       
    };

    private int currentLine = 0; // 現在表示しているシナリオの行番号

    void Start()
    {
        // 最初のセリフを表示
        ShowNextSentence();
    }

    void Update()
    {
        // マウスの左クリック、または画面タップで次のセリフへ
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextSentence();
        }
    }

    void ShowNextSentence()
    {
        // もし全てのセリフを表示し終わっていたら
        if (currentLine >= scenarios.Length)
        {
            // 次のシーンへ遷移する
            SceneManager.LoadScene(nextSceneName);
            return; // この後の処理は行わない
        }

        // テキストUIに現在の行のセリフを表示
        scenarioText.text = scenarios[currentLine];
        // 次の行へ
        currentLine++;
    }
}