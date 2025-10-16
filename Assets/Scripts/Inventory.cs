using UnityEngine;
using System.Collections.Generic;

// 所持アイテムとその数を管理するためのクラス
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
    public List<InventorySlot> items = new List<InventorySlot>();

    public void AddItem(ItemData item, int count = 1)
    {
        // すでに同じアイテムを持っているか探す
        InventorySlot existingItem = items.Find(slot => slot.item == item);

        if (existingItem != null)
        {
            // 持っていれば数を増やす
            existingItem.count += count;
        }
        else
        {
            // 持っていなければ新しく追加
            items.Add(new InventorySlot(item, count));
        }
        Debug.Log(item.itemName + " を " + count + "個手に入れた。");
        // TODO: インベントリUIを更新
    }

}
