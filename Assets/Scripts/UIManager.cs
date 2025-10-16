using UnityEngine;
using UnityEngine.UI; // Imageを使うために必要
using TMPro;          // TextMeshProを使うために必要

public class UIManager : MonoBehaviour
{ 
    // --- インスペクターから設定するUIパーツ ---
    public Image hpBarFill;
    public TextMeshProUGUI hpText;

    // 外部から呼び出されるHP更新用の関数
    public void UpdateHpUI(int currentHp, int maxHp)
    {
        // HPバーの Fill Amount を更新 (HPを0～1の割合に変換)
        hpBarFill.fillAmount = (float)currentHp / maxHp;

        // HPテキストを更新
        hpText.text = $"HP: {currentHp} / {maxHp}";
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
