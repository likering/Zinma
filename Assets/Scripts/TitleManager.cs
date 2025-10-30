using UnityEngine;
using UnityEngine.SceneManagement; // シーンを管理するために必要

public class TitleManager : MonoBehaviour
{
    // ボタンから呼び出すための公開メソッド
    public void StartGame()
    {
        // "GameScene"の部分は、あなたが作成したゲーム本編のシーン名に書き換えてください。
        // 例: "FieldScene", "Level1" など
        SceneManager.LoadScene("ScenarioScene");
    }
}
