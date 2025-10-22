// EquipmentData.cs (子クラス)
using UnityEngine;

// 装備のスロットを定義するenum（列挙型）
public enum EquipSlot
{
    NotEquippable, // 装備不可
    Weapon,        // 武器スロット
    Head,          // 頭スロット
    Body,          // 体スロット
    Accessory      // アクセサリー
}

// enum定義の下にクラスが来るようにする
[CreateAssetMenu(fileName = "NewEquipmentData", menuName = "RPG/Create Equipment Data")]
public class EquipmentData : ItemData // ← ItemDataを継承
{
   
   

    [Header("装備品情報")]
    [SerializeField] private EquipSlot equipSlot;
    [SerializeField] private int attackPower;
    [SerializeField] private int defensePower;

    public EquipSlot Slot => equipSlot;
    public int AttackPower => attackPower;
    public int DefensePower => defensePower;
}