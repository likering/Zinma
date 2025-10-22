// PlayerEquipment.cs
using UnityEngine;
using System.Collections.Generic; // Dictionaryを使うために必要

public class PlayerEquipment : MonoBehaviour
{
    // 現在の装備状態を保存する辞書（Dictionary）
    // Key: 装備スロット, Value: 装備データ
    public Dictionary<EquipSlot, EquipmentData> equippedItems = new Dictionary<EquipSlot, EquipmentData>();

    private PlayerStats playerStatus;
    private Inventory inventory; // インベントリの参照

    void Start()
    {
        playerStatus = GetComponent<PlayerStats>();
        // シーン内のどこかにあるInventoryスクリプトを探して取得
        inventory = FindObjectOfType<Inventory>();
    }

    // アイテムを装備するメソッド
    public void Equip(EquipmentData newItem)
    {
        EquipSlot slot = newItem.Slot;

        // 1. もし同じスロットに既に何か装備していたら、それを外す
        if (equippedItems.ContainsKey(slot))
        {
            Unequip(slot);
        }

        // 2. 新しいアイテムをスロットにセット
        equippedItems[slot] = newItem;
        Debug.Log(newItem.ItemName + " を " + slot + " に装備した。");

        // 3. ステータスを再計算するよう依頼
        playerStatus.UpdateStatus();
    }

    // アイテムを外すメソッド
    public void Unequip(EquipSlot slot)
    {
        // そのスロットに装備がなければ何もしない
        if (!equippedItems.ContainsKey(slot))
        {
            return;
        }

        // 1. 外すアイテムのデータを取得
        EquipmentData oldItem = equippedItems[slot];

        // 2. インベントリにアイテムを戻す（Inventory側にAddItemメソッドがあると仮定）
        inventory.AddItem(oldItem);
        Debug.Log(oldItem.ItemName + " をインベントリに戻した。");

        // 3. 装備リストから削除
        equippedItems.Remove(slot);

        // 4. ステータスを再計算するよう依頼
        playerStatus.UpdateStatus();
    }

    // 全装備の合計攻撃力ボーナスを計算して返す
    public int GetTotalAttackBonus()
    {
        int total = 0;
        foreach (EquipmentData item in equippedItems.Values)
        {
            total += item.AttackPower;
        }
        return total;
    }

    // 全装備の合計防御力ボーナスを計算して返す
    public int GetTotalDefenseBonus()
    {
        int total = 0;
        foreach (EquipmentData item in equippedItems.Values)
        {
            total += item.DefensePower;
        }
        return total;
    }
}
