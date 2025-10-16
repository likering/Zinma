using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MaterialCost
{
    public ItemData material;
    public int count;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "RPG/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<MaterialCost> requiredMaterials; // 必要な素材リスト
    public GameObject createdCompanionPrefab; // このレシピで生成される仲間のPrefab
    public ItemData createdItem; // または生成されるアイテム
}

