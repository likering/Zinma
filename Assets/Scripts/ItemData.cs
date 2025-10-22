using UnityEngine;


// アイテムの種類を定義するenum（列挙型）
public enum ItemType
{
    Weapon, // 武器
    Armor,  // 防具
    Potion, // ポーション
    Material // 素材
}

public class ItemData : ScriptableObject
{
    internal readonly Sprite itemIcon;
    [SerializeField] private string itemName;         // アイテム名
    [SerializeField] private ItemType itemType;       // アイテムの種類
    [SerializeField] private Sprite icon;             // アイコン
    [SerializeField][TextArea] private string description; // 説明文
    [SerializeField] private int power;               // 効果量（攻撃力や回復量など）
    [SerializeField] private int price;               // 価格

    // ゲッター（外部から値を取得するためのプロパティ）
    public string ItemName => itemName;
    public ItemType Type => itemType;
    public Sprite Icon => icon;
    public string Description => description;
    public int Power => power;
    public int Price => price;
}
