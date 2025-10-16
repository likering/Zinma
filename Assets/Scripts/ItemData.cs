using UnityEngine;

// createAssetMenu を使うと、Unityエディタの右クリックから簡単にデータアセットを作成できる
[CreateAssetMenu(fileName = "New ItemData", menuName = "RPG/Item Data")]
public class ItemData : MonoBehaviour
{

    public string itemName;
    public Sprite icon;
    [TextArea]
    public string description;
    // 必要に応じて、アイテムの種類（武器、素材、消費アイテムなど）をenumで定義すると便利


void Start()
    {
        
    }

    void Update()
    {
        
    }
}
