using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;

    // UIManagerへの参照を保持する変数
    private UIManager uiManager;

    [SerializeField]
    private float moveSpeed = 5.0f; // 左右移動の速さ
    [SerializeField]
    private float jumpForce = 8.0f; // ジャンプの力

    private Rigidbody rb;
    private bool isGrounded;

    [Header("攻撃設定")]
    [SerializeField]
    private Transform attackPoint; // 攻撃判定の中心位置
    [SerializeField]
    private float attackRange = 0.8f; // 攻撃の半径
    [SerializeField]
    private float attackDamage = 25f;  // 攻撃力
    [SerializeField]
    private LayerMask enemyLayers; // 敵のレイヤー

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector2 inputVec;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        controller = GetComponent<CharacterController>();
        // attackPointが設定されていなければプレイヤー自身を基点にする
        if (attackPoint == null)
        {
            attackPoint = this.transform;
        }

        currentHp = maxHp;

        // シーン内にあるUIManagerを探してきて、変数に格納する
        // (もっと良い方法はありますが、まずはこれで動かします)
        uiManager = FindObjectOfType<UIManager>();

        // 開始時にUIを初期化
        if (uiManager != null)
        {
            uiManager.UpdateHpUI(currentHp, maxHp);
        }
    }

    void Update()
    {
        // マウスの左クリックを検出
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        // 左右と前後の入力を取得
        float x = Input.GetAxis("Horizontal"); // A/Dキーまたは←/→キー
        float z = Input.GetAxis("Vertical");   // W/Sキーまたは↑/↓キー

        // 現在の速度を維持しつつ、左右と前後の速度を設定
        // カメラの向きに合わせて移動方向を計算
        Vector3 move = transform.right * x + transform.forward * z;
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);

        // ジャンプ処理
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        
    }
    void Attack()
    {
        // 画面中央から前方に向けてRay（光線）を飛ばす
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2));
        RaycastHit hit;

        // Rayが何かに当たったか？
        if (Physics.Raycast(ray, out hit, 100f)) // 100fは射程距離
        {
            Debug.Log("Rayが " + hit.collider.name + " に当たった！");

            // 当たった相手にEnemyControllerスクリプトが付いているか？
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 付いていれば、その敵のTakeDamageメソッドを呼び出す
                enemy.TakeDamage(10); // ここでは10ダメージを与える
            }
        }
      
    }

    // --- PlayerInputコンポーネントから"Attack"アクションが呼ばれたときに実行されるメソッドを追加 ---
    public void OnAttack(InputValue value)
    {
        // 攻撃アニメーションなどをここで再生する

        Debug.Log("攻撃！");

        // 攻撃範囲内にいる敵のColliderをすべて検出する
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // 検出したすべての敵にダメージを与える
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name + " にヒット！");
            // 敵からEnemyHealthコンポーネントを取得して、ダメージを与えるメソッドを呼び出す
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    // ダメージを受ける処理（外部から呼ばれることを想定）
    public void TakeDamage(int damage)
    {
        // 防御力の計算などをここで行う
        int actualDamage = damage; // - defensePower;
        if (actualDamage < 1) actualDamage = 1;

        currentHp -= damage;

        // HPが0未満にならないように調整
        if (currentHp < 0)
        {
            currentHp = 0;
        }

        Debug.Log("プレイヤーが " + damage + " のダメージを受けた！ 残りHP: " + currentHp);

        // UIManagerのHP更新関数を呼び出す
        if (uiManager != null)
        {
            uiManager.UpdateHpUI(currentHp, maxHp);
        }

        if (currentHp <= 0)
        {
            Die();
        }

        void Die()
        {
            Debug.Log("プレイヤーは力尽きた...");
            // ここにゲームオーバー処理などを追加
        }
    }

    // Sceneビューに攻撃範囲を視覚的に表示するためのギズモ
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // 地面に接触したときの処理
    void OnCollisionStay(Collision collisionInfo)
    {
        // "Ground"タグが付いたオブジェクトに接触しているか確認
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // 地面から離れたときの処理
    private void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }



}
