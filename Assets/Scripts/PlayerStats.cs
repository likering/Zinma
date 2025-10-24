using UnityEngine;
using UnityEngine.Events;
using System; // Actionを使うために必要


public class PlayerStats : MonoBehaviour
{
    // ★★★ 1. シングルトンインスタンスを追加 ★★★
    public static PlayerStats instance;

    [Header("レベルと経験値")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceForNextLevel = 100;

    [Header("基本ステータス")]
    public int maxHp = 100;
    public int currentHp;
    public int attackPower = 10;
    public int defensePower = 5; // 必要に応じて防御力なども追加

    [Header("最終ステータス")]
    public int CurrentAttack { get; private set; }
    public int CurrentDefense { get; private set; }

    [Header("関連コンポーネント")]
    // PlayerEquipmentコンポーネントを保持しておく変数
    private PlayerEquipment playerEquipment;
    //private UIManager uiManager; // UIを更新するための参照

    // イベントの型を <現在のHP, 最大HP> に変更
    public UnityEvent<int, int> OnHealthChanged;

    // ★ ステータスが変更されたことを通知するためのイベント
    public event Action OnStatusChanged;

    void Awake()
    {
        // ★★★ 2. シングルトンの設定を追加 ★★★
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // 自分と同じGameObjectについているPlayerEquipmentを取得
        playerEquipment = GetComponent<PlayerEquipment>();
        // ゲーム開始時にステータスを初期計算
        UpdateStatus();
    }

    void Start()
    {
        // 現在のHPを最大値で初期化
        currentHp = maxHp;

        

        // 開始時に一度だけUIを更新
        NotifyHpUpdate();
    }
    public void Heal(int amount)
    {
        // 回復後のHPを計算
        currentHp += amount;

        // もし最大HPを超えていたら、最大HPに補正する
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }

        Debug.Log("HPが " + amount + " 回復した！ 現在のHP: " + currentHp);

        // HPが変更されたことをUIなどに通知する
        NotifyHpUpdate();
    }

    // ★★★ ダメージ処理 (UI更新を一本化) ★★★
    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(1, damage - CurrentDefense); // 防御力を考慮する例

        currentHp -= actualDamage;
        if (currentHp < 0)
        {
            currentHp = 0;
        }
        Debug.Log("プレイヤーが " + actualDamage + " のダメージを受けた！ 残りHP: " + currentHp);
        // 変更を通知
        NotifyHpUpdate();

        if (currentHp <= 0)
        {
            Die();
        }
    }
    // ステータスを再計算するメソッド
    public void UpdateStatus()
    {
        int attackBonus = (playerEquipment != null) ? playerEquipment.GetTotalAttackBonus() : 0;
        int defenseBonus = (playerEquipment != null) ? playerEquipment.GetTotalDefenseBonus() : 0;

        CurrentAttack = attackPower + attackBonus;
        CurrentDefense = defensePower + defenseBonus;
        Debug.Log("ステータス更新: 攻撃力=" + CurrentAttack + ", 防御力=" + CurrentDefense);

        // ステータス全体の変更を通知
        OnStatusChanged?.Invoke();

        // HPにも影響がある可能性があるので、HPの通知も行う
        if (currentHp > maxHp) currentHp = maxHp;
        NotifyHpUpdate();
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
        Debug.Log("最大HP: " + maxHp + ", 攻撃力: " + attackPower + ", 防御力: " + defensePower + "に上がった ");

        UpdateStatus();
    }
    // UIを更新
    private void NotifyHpUpdate()
    {
        // 1. イベントを発行する (HPバーなど、他のスクリプトがこれを受け取る)
        OnHealthChanged.Invoke(currentHp, maxHp);

        // ★★★ 修正後 ★★★
        // UIManager.instance で直接シングルトンを呼び出す
        if (UIManager.instance != null)
        {
            // UIManagerが要求する2つの引数を渡す
            UIManager.instance.UpdateHpUI(this.currentHp, this.maxHp);
        }
        else
        {
            Debug.LogWarning("PlayerStats: UIManager.instance が見つかりません。");
        }
    }




    private void Die()
    {
        Debug.Log("プレイヤーは力尽きた...");
        // ゲームオーバー処理
    }

    
}
