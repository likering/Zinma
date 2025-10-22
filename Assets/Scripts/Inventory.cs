// Inventory.cs
using UnityEngine;
using System.Collections.Generic;

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
    // 【追記】テスト用アイテムをインスペクターから設定するための変数
    [SerializeField] private ItemData testItem1;
    [SerializeField] private ItemData testItem2;

    public List<InventorySlot> items = new List<InventorySlot>();

    // 【追記1】PlayerEquipmentへの参照を保持する変数
    public PlayerEquipment playerEquipment;

    // 【追記2】StartメソッドでPlayerEquipmentを自動で見つけてくる
    void Start()
    {
        // 【追記】ゲーム開始時にテスト用アイテムを追加する
        if (testItem1 != null) { AddItem(testItem1); }
        if (testItem2 != null) { AddItem(testItem2); }

        // シーン内からPlayerEquipmentコンポーネントを探してきて、変数に格納する
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        if (playerEquipment == null)
        {
            Debug.LogError("シーン内にPlayerEquipmentが見つかりませんでした。Playerにアタッチされているか確認してください。");
        }
    }

    public void AddItem(ItemData item, int count = 1)
    {
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
        // TODO: インベントリUIを更新
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
            // 装備したので、インベントリからアイテムを1つ減らす
            RemoveItem(item, 1);
        }
        else
        {
            // ★★★ デバッグ用のログを追加 ★★★
            Debug.Log("このアイテムは EquipmentData ではありません。消費アイテムとして処理します。");

            // 装備品でなければ、ポーションなどの消費アイテムとして扱う
            Debug.Log(item.ItemName + " を使った！（ここに回復などの処理を記述）");
            // 使ったので、インベントリからアイテムを1つ減らす
            RemoveItem(item, 1);
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
        // TODO: インベントリUIを更新
    }
}