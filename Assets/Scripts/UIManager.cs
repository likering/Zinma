using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Imageを使うために必要

public class UIManager : MonoBehaviour
{
    // ★★★ 1. シングルトンインスタンスを追加 ★★★
    public static UIManager instance;

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

    // ★★★ 2. Awakeメソッドを追加 ★★★
    void Awake()
    {
        // シングルトンの設定
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            // 既にインスタンスが存在する場合は、重複しないように自分を破棄する
            Destroy(gameObject);
        }
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            // ★★★ 連携の核心 ★★★
            // PlayerStatsのステータスが更新されたら、UpdateStatusUIを呼ぶようにイベントを登録
            playerStats.OnStatusChanged += UpdateStatusUI;
           

        }
        
    }
    // 外部から呼び出されるHP更新用の関数
    public void UpdateHpUI(int currentHp, int maxHp)
    {
        // HPバーの Fill Amount を更新
        if (hpBarFill != null)
            hpBarFill.fillAmount = (float)currentHp / maxHp;

        // HPテキストを更新 (レベル表示が必要ならPlayerStatsから別途取得)
        if (hpText != null && PlayerStats.instance != null)
        {
            // PlayerStatsがシングルトンなら PlayerStats.instance.currentLevel で取得できる
            // そうでなければ、playerStats 変数から取得する
            hpText.text = $"Lv: {playerStats.currentLevel}  HP: {currentHp} / {maxHp}";
        }
    }
    void Start()
    {
        // 最初はUIを非表示にしておく
        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(false);
        }
        UpdateStatusUI(); // 表示する瞬間に必ず最新情報に更新

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
