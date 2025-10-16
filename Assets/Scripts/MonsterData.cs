using UnityEngine;
using System.Collections.Generic;

// アイテムのドロップ確率を管理するクラス
[System.Serializable]

public class LootItem
{
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
}
