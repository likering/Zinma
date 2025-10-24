using Assets.SimpleSpriteTrails.Scripts;
using UnityEngine;

// PlayerStatsコンポーネントが同じオブジェクトにないとエラーになるようにする
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    // --- PlayerStatsへの参照を保持する変数 ---
    private PlayerStats playerStats;
    private Animator animator; // Animatorコンポーネントへの参照

    [SerializeField]
    private float moveSpeed = 5.0f; // 左右移動の速さ
    [SerializeField]
    private float jumpForce = 8.0f; // ジャンプの力

    private Rigidbody rb;
    private bool isGrounded;

    [Header("攻撃設定")]
    [SerializeField]
    private Transform attackPoint; // 攻撃判定の中心位置

    // ※ attackRange, attackDamage, enemyLayers は OnAttack メソッドで使われているため残します
    [SerializeField]
    private float attackRange = 1.0f; // 攻撃の半径

    // ※ OnAttackメソッドで使われているため、以下の2つも必要です
    [SerializeField]
    private float attackDamage = 25f;  // 攻撃力
    [SerializeField]
    private LayerMask enemyLayers; // 敵のレイヤー

    // インスペクターから斬撃エフェクトのParticle Systemをアタッチする
    public ParticleSystem slashEffect;
    // ① 音を鳴らすためのAudioSourceコンポーネントを格納する変数
    public AudioSource audioSource;

    public AudioClip attackSound;

    public MeleeWeaponTrail weaponTrail;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ★★★ 修正点 ★★★
        // 自分に付いている PlayerStats コンポーネントを取得して、変数に保存する
        playerStats = GetComponent<PlayerStats>();
        audioSource = GetComponent<AudioSource>();
        // 自分に付いている Animator コンポーネントを取得
        animator = GetComponent<Animator>();



        // attackPointが設定されていなければプレイヤー自身を基点にする (これはOK)
        if (attackPoint == null)
        {
            attackPoint = this.transform;
        }

        // --- HPやUIの初期化処理は PlayerStats が行うので、ここからは削除 ---
    }

    void Update()
    {
        // マウスの右クリックを検出
        if (Input.GetMouseButtonDown(1))
        {
            // アニメーショントリガー名: "Attack", ダメージ倍率: 1.0f (通常倍率)
            PerformAttack("Attack", 1.0f);
        }

        // マウスの左クリックを検出
        if (Input.GetMouseButtonDown(0))
        {
            // PerformAttack を呼び出す
            // アニメーショントリガー名: "Attack2", ダメージ倍率: 2.0f (2倍)
            PerformAttack("Attack2", 2.0f);
        }
        // --- 移動処理 (ここは変更なし) ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

        // 移動ベクトル（move）の長さ（magnitude）が0より大きいかどうかで移動中か判定
        // 0.1fとしているのは、入力の微細なブレを無視するため
        bool isMoving = move.magnitude > 0.1f;


        // Animatorに移動状態を伝える
        if (animator != null)
        {
            // "IsMoving"という名前のBoolパラメータに、判定結果(trueかfalse)をセットする
            animator.SetBool("IsMoving", isMoving);
        }
        else
        {
            // animatorがnullの場合に警告を出す
            Debug.LogWarning("Animatorが見つかりません！");
        }

        // --- ジャンプ処理 (ここは変更なし) ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // テスト用にスペースキーでダメージを受ける

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(15);
        }


        // ★★★ (任意) 移動アニメーションの制御 ★★★
        // animator.SetFloat("Speed", move.magnitude);
    }

    // ★★★ 新しい統合された攻撃メソッド ★★★
    void PerformAttack(string triggerName, float damageMultiplier)
    {
        // 1. 指定されたアニメーショントリガーを起動
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        // 2. 攻撃判定 (Raycast)
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // エフェクトを再生
        if (slashEffect != null)
        {
            slashEffect.Play();
        }

        // 攻撃が当たったかどうかの判定 (Rayの距離なども攻撃によって変えても良い)
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Rayが " + hit.collider.name + " に当たった！");

            if (audioSource != null && attackSound != null)
                audioSource.PlayOneShot(attackSound);

            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 3. ダメージ計算
                // プレイヤーの基本攻撃力 (装備補正込み) を取得
                int baseAttackPower = playerStats.CurrentAttack;

                // 基本攻撃力に、この攻撃の倍率を掛ける
                // (int)でキャストして、最終的なダメージを整数にする
                int finalDamage = (int)(baseAttackPower * damageMultiplier);

                Debug.Log($"{triggerName} を実行！ 基本攻撃力: {baseAttackPower}, 倍率: {damageMultiplier}, 最終ダメージ: {finalDamage}");

                // 4. 敵にダメージを与える
                enemy.TakeDamage(finalDamage);
            }
        }
    }
    
    // --- この OnAttack メソッドは PlayerInput システムを使っている場合の攻撃処理です。
    // --- Update内のマウス入力とどちらを使うか、後で統一する必要があります。
    public void OnAttack(UnityEngine.InputSystem.InputValue value)
    {
        // ... (この中身は一旦そのまま)
    }

    // ダメージを受ける処理は、PlayerStatsに処理を依頼するだけにする
    public void TakeDamage(int damage)
    {
        // PlayerStatsのTakeDamageメソッドを呼び出す
        playerStats.TakeDamage(damage);
    }



    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    public void GenerateTrail()
    {
        if (weaponTrail != null)
        {
            weaponTrail.Build();
        }
    }
}