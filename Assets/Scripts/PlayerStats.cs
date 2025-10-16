using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("レベルと経験値")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceForNextLevel = 100;

    [Header("基本ステータス")]
    public int maxHp = 100;
    public int currentHp;
    public int attackPower = 10;
    public int defensePower = 5; // 必要に応じて防御力なども追加

    [Header("関連コンポーネント")]
    private UIManager uiManager; // UIを更新するための参照

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

    // 経験値を獲得する
    public void GainExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log(amount + " の経験値を獲得！ 現在の経験値: " + currentExperience);

        while (currentExperience >= experienceForNextLevel)
        {
            LevelUp();
        }
    }

    // レベルアップ処理
    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceForNextLevel;

        // --- ここでステータスを更新 ---
        // 例：最大HPが20、攻撃力が5上昇する
        maxHp += 20;
        attackPower += 5;
        defensePower += 1;

        // HPを全回復させる
        currentHp = maxHp;

        // 次のレベルアップに必要な経験値を設定（例：1.2倍にする）
        experienceForNextLevel = Mathf.RoundToInt(experienceForNextLevel * 1.2f);

        Debug.Log("レベルアップ！ レベル " + currentLevel + " になった！");
        Debug.Log("最大HP: " + maxHp + ", 攻撃力: " + attackPower+", 防御力: " + defensePower +"に上がった " );

        // UIを更新
        if (uiManager != null)
        {
            uiManager.UpdateHpUI(currentHp, maxHp);
            // uiManager.UpdateLevelUI(currentLevel); // レベル表示用のUIメソッドがあれば呼び出す
        }
    }

    // ダメージを受ける処理
    public void TakeDamage(int damage)
    {
        // 防御力の計算などをここで行う
        int actualDamage = damage; // - defensePower;
        if (actualDamage < 1) actualDamage = 1;

        currentHp -= actualDamage;
        if (currentHp < 0) currentHp = 0;

        Debug.Log("プレイヤーが " + actualDamage + " のダメージを受けた！ 残りHP: " + currentHp);

        // UIを更新
        if (uiManager != null)
        {
            uiManager.UpdateHpUI(currentHp, maxHp);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("プレイヤーは力尽きた...");
        // ゲームオーバー処理
    }
}
