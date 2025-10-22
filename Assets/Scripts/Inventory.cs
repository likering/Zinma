using UnityEngine;
using System.Collections.Generic;
using System; // Action を使うために必要

// 所持アイテムとその数を管理するためのクラス
// (この部分は変更なし)
public class InventorySlot
{
    public ItemData item;
    public int count;

    public InventorySlot(ItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

public class Inventory : MonoBehaviour
{
    private PlayerStats playerStats;
    private UIManager uiManager;
    // ★ どこからでもアクセスできる静的なインスタンス
    public static Inventory instance;

    // ★ アイテムリストが変更されたことをUIに通知するためのイベント
    public event Action OnInventoryChanged;

    public List<InventorySlot> items = new List<InventorySlot>();

    // 【追記1】PlayerEquipmentへの参照を保持する変数
    public PlayerEquipment playerEquipment;

    // 【追記2】StartメソッドでPlayerEquipmentを自動で見つけてくる

    void Awake()
    { 
        // ★ オブジェクトのIDをログに出して、どのオブジェクトかを特定できるようにする
        Debug.Log($"Inventory.Awake() 実行中 on GameObject: {gameObject.name}, InstanceID: {gameObject.GetInstanceID()}");

        // ★ シングルトンの設定
        if (instance == null)
        {
            // まだ誰もインスタンスになっていなければ、自分がなる
            instance = this;
            // (任意) シーンをまたいでインベントリを維持したい場合
            // DontDestroyOnLoad(gameObject);
        }
        else if(instance != this) // ← この条件が重要
        {
            // ★★★ 追跡用ログ ★★★
            Debug.LogError($"シングルトン競合！ '{this.gameObject.name}' (InstanceID: {this.gameObject.GetInstanceID()}) は、既存の '{instance.gameObject.name}' (InstanceID: {instance.gameObject.GetInstanceID()}) があるため破棄されます。");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        // シーン内からPlayerEquipmentコンポーネントを探してきて、変数に格納する
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        if (playerEquipment == null)
        {
            Debug.LogError("シーン内にPlayerEquipmentが見つかりませんでした。Playerにアタッチされているか確認してください。");
        }
    }

    public void AddItem(ItemData item, int count = 1)
    {
        Debug.Log($"--- AddItem 実行 on InstanceID: {gameObject.GetInstanceID()} ---");
        // (この部分は変更なし)
        InventorySlot existingItem = items.Find(slot => slot.item == item);

        if (existingItem != null)
        {
            existingItem.count += count;
        }
        else
        {
            items.Add(new InventorySlot(item, count));
        }
        Debug.Log(item.ItemName + " を " + count + "個手に入れた。");
        // ★ 処理の最後にイベントを呼び出す
        OnInventoryChanged?.Invoke();
    }

    // 【追記3】アイテムを消費・装備するためのメソッド
    // UIのボタンなどから、選択したアイテムのItemDataを引数にして呼び出すことを想定
    public void UseItem(ItemData item)
    {
        // ★★★ デバッグ用のログを追加 ★★★
        Debug.Log("--- UseItemメソッドが呼ばれました！ 渡されたアイテム: " + item.ItemName + " ---");

        // is演算子で、渡されたアイテムが装備品(EquipmentData)かどうかを判別
        if (item is EquipmentData equipment)
        {
            // ★★★ デバッグ用のログを追加 ★★★
            Debug.Log("このアイテムは EquipmentData として認識されました。装備処理を開始します。");

            // 装備品だった場合、PlayerEquipmentに装備処理を依頼
            playerEquipment.Equip(equipment);
            // ★ 装備音を再生
            AudioManager.instance.PlaySE(AudioManager.instance.itemEquippedSound);

            // 装備したので、インベントリからアイテムを1つ減らす
            RemoveItem(item, 1);
        }

        if (item.Type == ItemType.Potion)
        {
            // ポーションだった場合、プレイヤーのHPを回復
            playerStats.Heal(item.Power);
            Debug.Log(item.ItemName + " を使ってHPが " + item.Power + " 回復した！");
            uiManager.ShowMessage($"{item.ItemName} を使った！\nHPが {item.Power} 回復した！");
            // ★ アイテム使用音を再生
            AudioManager.instance.PlaySE(AudioManager.instance.itemUsedSound);
            RemoveItem(item, 1);


        }
        else
        {
            // ★★★ デバッグ用のログを追加 ★★★
            Debug.Log("このアイテムは EquipmentData ではありません。消費アイテムとして処理します。");

            
        }
    }


    // 【追記4】インベントリから指定されたアイテムを減らすメソッド
    public void RemoveItem(ItemData item, int count = 1)
    {
        // 減らす対象のアイテムを探す
        InventorySlot targetSlot = items.Find(slot => slot.item == item);

        if (targetSlot == null)
        {
            Debug.LogWarning(item.ItemName + " はインベントリにないため、減らせません。");
            return;
        }

        // 個数を減らす
        targetSlot.count -= count;

        // もしアイテムの個数が0以下になったら、リストから完全に削除する
        if (targetSlot.count <= 0)
        {
            items.Remove(targetSlot);
        }

        Debug.Log(item.ItemName + " を " + count + "個消費/装備しました。");
        // ★ 処理の最後にイベントを呼び出す
        OnInventoryChanged?.Invoke();
    }

    // ★ オブジェクトが破棄される時に呼ばれる
    void OnDestroy()
    {
        Debug.LogError($"!!! Inventoryが破棄されました on GameObject: {gameObject.name}, InstanceID: {gameObject.GetInstanceID()} !!!");
    }

    
}