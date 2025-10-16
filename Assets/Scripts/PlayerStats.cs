using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("レベルと経験値")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("基本ステータス")]
    public int maxHp = 100;
    public int currentHp;
    public int attackPower = 10;
    public int defensePower = 5; // 必要に応じて防御力なども追加

    [Header("関連コンポーネント")]
    private UIManager uiManager; // UIを更新するための参照

    public void GainExperience(int amount)
    {
        currentExp += amount;
        Debug.Log(amount + " の経験値を獲得！ 現在の経験値: " + currentExp);

        // 経験値が次のレベルに達したらレベルアップ
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }
    void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;

        // 次のレベルアップに必要な経験値を増やす（例: 1.5倍）
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.5f);

        Debug.Log("レベルアップ！ Lv." + level + " になった！");
        // TODO: UIの更新やステータスアップの処理
        // 例：最大HPが20、攻撃力が5上昇する
        maxHp += 20;
        attackPower += 5;
        defensePower += 1;

        // HPを全回復させる
        currentHp = maxHp;

        Debug.Log("最大HP: " + maxHp + ", 攻撃力: " + attackPower + "防御力:" + defensePower + "に上がった");

        // UIを更新
        if (uiManager != null)
        {
            uiManager.UpdateHpUI(currentHp, maxHp);
            // uiManager.UpdateLevelUI(currentLevel); // レベル表示用のUIメソッドがあれば呼び出す
        }
    }


    void Start()
    {
        // 現在のHPを最大値で初期化
        currentHp = maxHp;

        // UIManagerを探して参照を保持
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            // 開始時にUIを初期化
            uiManager.UpdateHpUI(currentHp, maxHp);
            // uiManager.UpdateLevelUI(currentLevel); // レベル表示用のUIメソッドがあれば呼び出す
        }
    }

    void Update()
    {

    }
}
