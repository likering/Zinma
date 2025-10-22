using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory playerInventory; // InspectorからプレイヤーのInventoryを設定

    [Header("UIパーツ")]
    public GameObject inventoryPanel;
    public Transform itemContent; // ScrollViewのContent
    public GameObject inventoryItemPrefab; // リストに並べるアイテムボタンのプレハブ

    public GameObject itemDetailPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    public GameObject commandPanel;
    public Button useButton;
    public Button equipButton;


    private InventorySlot selectedSlot; // ★ ItemData ではなく InventorySlot で保持する

    void Start()
    {

        // ボタンのリスナーを、新しい統一メソッドに登録し直す
        useButton.onClick.AddListener(OnActionButtonClicked);
        equipButton.onClick.AddListener(OnActionButtonClicked);

        // Inventoryのイベントに関数を登録する
        playerInventory.OnInventoryChanged += UpdateUI;
    }
    void OnDestroy()
    {
        // オブジェクトが破棄される時に、登録を解除する（お作法）
        playerInventory.OnInventoryChanged -= UpdateUI;
    }

    // 「つかう」でも「そうび」でも、このメソッドを呼び出すように統一する
    void OnActionButtonClicked()
    {
        // 選択されているスロットがなければ何もしない
        if (selectedSlot == null) return;

        // ★★★ 連携の核心部分 ★★★
        // Inventory.cs の UseItem メソッドを、選択されているアイテムのデータで呼び出す
        playerInventory.UseItem(selectedSlot.item);

        // 処理が終わったらUIを更新する
        UpdateUI();

        // 詳細パネルなどを非表示にする
        itemDetailPanel.SetActive(false);
        commandPanel.SetActive(false);
        selectedSlot = null; // 選択状態をリセット
    }

    // インベントリUIを開く/閉じる処理 (以前作成したUIManagerなどから呼ばれる想定)
    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inventoryPanel.activeSelf)
        {
            UpdateUI();
        }
    }

    // UIのリストを更新する
    void UpdateUI()
    {
        // 既存のアイテムリストを一旦すべて削除
        foreach (Transform child in itemContent)
        {
            Destroy(child.gameObject);
        }
        // インベントリのアイテムスロットを元にリストを再生成
        foreach (InventorySlot slot in playerInventory.items)
        {

            GameObject itemObj = Instantiate(inventoryItemPrefab, itemContent);

            // ボタンのテキストにアイテム名と個数を表示
            itemObj.GetComponentInChildren<TextMeshProUGUI>().text = $"{slot.item.ItemName} x{slot.count}";

            // ★★★ 最重要ポイント ★★★
            // このボタンがクリックされたら、OnItemSelectedメソッドを、このslotの情報で呼び出すように設定
            itemObj.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(slot));
        }
    }

    // ★★★ アイテムリストのボタンがクリックされた時に呼ばれる ★★★
    void OnItemSelected(InventorySlot slot)
    {
        // 選択されたスロットをクラス内の変数に保存
        selectedSlot = slot;

        // 詳細パネルやコマンドパネルを表示する処理
        itemDetailPanel.SetActive(true);
        commandPanel.SetActive(true);

        // 詳細情報を更新
        itemNameText.text = slot.item.ItemName;
        itemDescriptionText.text = slot.item.Description;
        itemIcon.sprite = slot.item.itemIcon;

        // アイテムの種類に応じて「そうび」ボタンなどを表示/非表示にする
        bool isEquipment = (slot.item.Type == ItemType.Weapon || slot.item.Type == ItemType.Armor);
        equipButton.gameObject.SetActive(isEquipment);
        useButton.gameObject.SetActive(slot.item.Type == ItemType.Potion); // ポーションの時だけ「つかう」
    }

  
}