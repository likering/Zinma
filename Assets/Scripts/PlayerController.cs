using UnityEngine;

// PlayerStatsコンポーネントが同じオブジェクトにないとエラーになるようにする
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    // --- PlayerStatsへの参照を保持する変数 ---
    private PlayerStats playerStats;

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
    private float attackRange = 0.8f; // 攻撃の半径

    // ※ OnAttackメソッドで使われているため、以下の2つも必要です
    [SerializeField]
    private float attackDamage = 25f;  // 攻撃力
    [SerializeField]
    private LayerMask enemyLayers; // 敵のレイヤー


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ★★★ 修正点 ★★★
        // 自分に付いている PlayerStats コンポーネントを取得して、変数に保存する
        playerStats = GetComponent<PlayerStats>();

        // attackPointが設定されていなければプレイヤー自身を基点にする (これはOK)
        if (attackPoint == null)
        {
            attackPoint = this.transform;
        }

        // --- HPやUIの初期化処理は PlayerStats が行うので、ここからは削除 ---
    }

    void Update()
    {
        // マウスの左クリックを検出
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        // --- 移動処理 (ここは変更なし) ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

        // --- ジャンプ処理 (ここは変更なし) ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Rayが " + hit.collider.name + " に当たった！");

            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // ★★★ 修正点 ★★★
                // PlayerStats が持っている攻撃力(attackPower)を使ってダメージを与える
                enemy.TakeDamage(playerStats.attackPower);
            }
        }
    }

    // --- この OnAttack メソッドは PlayerInput システムを使っている場合の攻撃処理です。
    // --- Update内のマウス入力とどちらを使うか、後で統一する必要があります。
    public void OnAttack(UnityEngine.InputSystem.InputValue value)
    {
        // ... (この中身は一旦そのまま)
    }

    // ★★★ 修正点 ★★★
    // ダメージを受ける処理は、PlayerStatsに処理を依頼するだけにする
    public void TakeDamage(int damage)
    {
        // PlayerStatsのTakeDamageメソッドを呼び出す
        playerStats.TakeDamage(damage);
    }


    // --- 以下のメソッドは変更なし ---

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
}