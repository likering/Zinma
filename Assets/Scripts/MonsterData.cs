using UnityEngine;
using System.Collections.Generic;

// アイテムのドロップ確率を管理するクラス
[System.Serializable]

public class LootItem
{
    internal readonly float dropRate;
    internal readonly ItemData itemData;
    public ItemData item;
    [Range(0, 100)]
    public float dropChance; // ドロップ確率 (0-100%)
}

[CreateAssetMenu(fileName = "New MonsterData", menuName = "RPG/Monster Data")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public int maxHp;
    public int attackPower;
    public int experiencePoint; // 倒した時に得られる経験値

    public List<LootItem> lootTable; // このモンスターがドロップする可能性のあるアイテムリスト
    [Range(0, 1)] // 確率を0%～100% (0.0～1.0) のスライダーで設定できるようにする
    public float dropRate;

    // ドロップするアイテムを抽選して返すメソッド
    public List<ItemData> GetDropItems()
    {
        List<ItemData> droppedItems = new List<ItemData>();

        // リストにある全てのアイテムについて、ドロップするかどうかを確率で判定する
        foreach (var item in lootTable)
        {
            // 0.0から1.0の間のランダムな値を生成し、設定した確率より小さいならドロップ成功
            if (Random.value <= item.dropRate)
            {
                droppedItems.Add(item.itemData);
            }
        }

        return droppedItems;
    }
}
