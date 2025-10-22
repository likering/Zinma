using UnityEngine;
using UnityEngine.Events;

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

    // --- 最終的なステータス（基本値 + 装備補正） ---
    public int CurrentAttack { get; private set; }
    public int CurrentDefense { get; private set; }

    // PlayerEquipmentコンポーネントを保持しておく変数
    private PlayerEquipment playerEquipment;

    [Header("関連コンポーネント")]
    private UIManager uiManager; // UIを更新するための参照

    // イベントの型を <現在のHP, 最大HP> に変更
    public UnityEvent<int, int> OnHealthChanged;

    void Awake()
    {
        // 自分と同じGameObjectについているPlayerEquipmentを取得
        playerEquipment = GetComponent<PlayerEquipment>();
        // ゲーム開始時にステータスを初期計算
        UpdateStatus();
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
            uiManager.UpdateHpUIText(currentHp, maxHp);
            // uiManager.UpdateLevelUI(currentLevel); // レベル表示用のUIメソッドがあれば呼び出す
        }


    }

    // ステータスを再計算するメソッド
    public void UpdateStatus()
    {
        // 装備の合計補正値を取得
        int attackBonus = playerEquipment.GetTotalAttackBonus();
        int defenseBonus = playerEquipment.GetTotalDefenseBonus();

        // 基本値と装備補正を合計して最終的なステータスを計算
        CurrentAttack = attackPower + attackBonus;
        CurrentDefense = defensePower + defenseBonus;

        Debug.Log("ステータス更新: 攻撃力=" + CurrentAttack + ", 防御力=" + CurrentDefense);
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

        UpdateStatus();

        // UIを更新
        if (uiManager != null)
        {
            uiManager.UpdateHpUIText(currentHp, maxHp);
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

        // ↓ このログがコンソールに出るか確認
        Debug.Log($"HP変更イベント発行: 現在HP={currentHp}, 最大HP={maxHp}");
        OnHealthChanged.Invoke(currentHp, maxHp);


        // UIを更新
        if (uiManager != null)
        {
            uiManager.UpdateHpUIText(currentHp, maxHp);
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
