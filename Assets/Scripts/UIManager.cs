using System;          // TextMeshProを使うために必要
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Imageを使うために必要

public class UIManager : MonoBehaviour
{
    // --- インスペクターから設定するUIパーツ ---
    public Image hpBarFill;
    public TextMeshProUGUI hpText;

    [Header("ステータスUIパーツ")]
    public GameObject statusPanel;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;

    [Header("メッセージウィンドウ")]
    public GameObject messageWindowPanel;
    public TextMeshProUGUI messageText;

    private PlayerStats playerStats; // PlayerStatsへの参照

    // Inspectorから装備品UIのパネルを割り当てる
    public GameObject equipmentPanel;
    // Inspectorからカメラのオブジェクトを割り当てる
    public CameraCotroller cameraController;
    public InventoryUI inventoryUI;


    // 外部から呼び出されるHP更新用の関数
    public void UpdateHpUIText(int currentLevel,int currentHp, int maxHp )
    {
        // ↓ このログがコンソールに出るか確認
        Debug.Log($"UI更新メソッド受信: 現在HP={currentHp}");

        // HPバーの Fill Amount を更新 (HPを0～1の割合に変換)
        hpBarFill.fillAmount = (float)currentHp / maxHp;

        // HPテキストを更新
        hpText.text = $"Lv: {currentLevel}  HP: {currentHp} / {maxHp}";
    }
    void Start()
    {
        // 最初はUIを非表示にしておく
        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(false);
        }
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            // ★★★ 連携の核心 ★★★
            // PlayerStatsのステータスが更新されたら、UpdateStatusUIを呼ぶようにイベントを登録
            playerStats.OnStatusChanged += UpdateStatusUI;
        }
        statusPanel.SetActive(false); // 最初は非表示
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

        // Cキーでステータスパネルの表示/非表示を切り替え
        if (Input.GetKeyDown(KeyCode.C))
        {
            statusPanel.SetActive(!statusPanel.activeSelf);
            if (statusPanel.activeSelf)
            {
                UpdateStatusUI(); // 表示する瞬間に必ず最新情報に更新
            }
        }

        // Input.GetKeyDownは、キーが「押された瞬間」に一度だけtrueになる
        // 例：Iキー、または Tabキーが押されたら
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            // InventoryUIスクリプトが設定されていれば
            if (inventoryUI != null)
            {
                // InventoryUIのToggleInventory()メソッドを呼び出す
                inventoryUI.ToggleInventory();
            }
        }
    }
    // ステータスUIを更新するメソッド
    public void UpdateStatusUI()
    {
        if (playerStats == null) return;

        attackText.text = $"攻撃力: {playerStats.CurrentAttack}";
        defenseText.text = $"防御力: {playerStats.CurrentDefense}";
    }

    // 外部からメッセージ表示を依頼するためのメソッド
    public void ShowMessage(string message)
    {
        // 既存のメッセージ表示コルーチンが動いていたら停止する
        StopAllCoroutines();
        StartCoroutine(ShowMessageCoroutine(message));
    }

    // メッセージを一定時間表示して消すコルーチン
    private IEnumerator ShowMessageCoroutine(string message)
    {
        // 1. ウィンドウを表示してテキストを設定
        messageWindowPanel.SetActive(true);
        messageText.text = message;

        // 2. 指定した秒数だけ待つ (例: 3秒)
        yield return new WaitForSeconds(3.0f);

        // 3. ウィンドウを非表示にする
        messageWindowPanel.SetActive(false);
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
