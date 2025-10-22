using UnityEngine;
using UnityEngine.UI; // Imageを使うために必要
using TMPro;          // TextMeshProを使うために必要

public class UIManager : MonoBehaviour
{ 
    // --- インスペクターから設定するUIパーツ ---
    public Image hpBarFill;
    public TextMeshProUGUI hpText;

    // Inspectorから装備品UIのパネルを割り当てる
    public GameObject equipmentPanel;
    // Inspectorからカメラのオブジェクトを割り当てる
    public CameraCotroller cameraController;

    // 外部から呼び出されるHP更新用の関数
    public void UpdateHpUIText(int currentHp, int maxHp)
    {
        // ↓ このログがコンソールに出るか確認
        Debug.Log($"UI更新メソッド受信: 現在HP={currentHp}");

        // HPバーの Fill Amount を更新 (HPを0～1の割合に変換)
        hpBarFill.fillAmount = (float)currentHp / maxHp;

        // HPテキストを更新
        hpText.text = $"HP: {currentHp} / {maxHp}";
    }
    void Start()
    {
        // 最初はUIを非表示にしておく
        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 例：Eキーを押したらUIの表示/非表示を切り替える
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 現在UIが表示されているかどうかで処理を分岐
            if (equipmentPanel.activeSelf)
            {
                CloseEquipmentUI();
            }
            else
            {
                OpenEquipmentUI();
            }
        }
    }

    public void OpenEquipmentUI()
    {
        equipmentPanel.SetActive(true);
        // カメラを停止し、カーソルを表示する
        cameraController.UnlockCursor();
    }

    public void CloseEquipmentUI()
    {
        equipmentPanel.SetActive(false);
        // カメラを再開し、カーソルをロックする
        cameraController.LockCursor();
    }
}
