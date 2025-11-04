using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;


public class BossController : MonoBehaviour
{
    // ★ Inspectorから対応するモンスターのデータを設定
    public MonsterData monsterData;

    private int enemyHP;
    private PlayerStats playerStats; // プレイヤーのステータス管理スクリプト
    private bool isDead;
    private bool isDamage;

    public GameObject Body; // 点滅させるメッシュを持つGameObject
    private bool isInvincible = false;

    public AudioSource audioSource;
    public AudioClip seDamage;
    public AudioClip seDeath;

    [Header("ドロップ設定")]
    [SerializeField] private IDroppable dropTable; // この敵が使用するドロップテーブル
    [SerializeField] private GameObject droppedItemPrefab; // ステップ3で作成したプレハブ

    private NavMeshAgent agent; // NavMeshAgentコンポーネントを格納する変数
    private Transform playerTransform; // プレイヤーのTransformを格納する変数
    private Animator animator; // ★ Animatorコンポーネントを格納する変数を追加

    [Header("AI設定")] 
    [SerializeField] private float detectionRange = 15f; // プレイヤーを検知する範囲
    [SerializeField] private float attackRange = 2f;    // 攻撃するために停止する距離


    [Header("攻撃設定")]
    [SerializeField] private int attackPower = 10; // 敵の攻撃力
    [SerializeField] private float attackCooldown = 2.0f; // 攻撃のクールダウンタイム（秒）
    private bool canAttack = true; // 攻撃可能かどうかを判定するフラグ


    void Start()
    {
        // MonsterDataからHPを初期設定
        if (monsterData != null)
        {
            enemyHP = monsterData.maxHp;
        }



        // プレイヤーのステータス管理コンポーネントを探しておく
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerStats = playerObj.GetComponent<PlayerStats>();
            // プレイヤーのTransformを取得して、追跡ターゲットに設定する
            playerTransform = playerObj.transform;

        }

        // このゲームオブジェクトに付いている AudioSource コンポーネントを取得して、
        // audioSource 変数に代入する
        audioSource = GetComponent<AudioSource>();

        // NavMeshAgentコンポーネントを取得
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("このGameObjectにNavMeshAgentがアタッチされていません: " + this.gameObject.name);
        }

        // ★ Animatorコンポーネントを取得
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.stoppingDistance = attackRange;
        }

        if (agent == null)
        {
            Debug.LogError("このGameObjectにNavMeshAgentがアタッチされていません: " + this.gameObject.name);
        }
        // ★ Animatorの存在もチェック
        if (animator == null)
        {
            Debug.LogError("このGameObjectにAnimatorがアタッチされていません: " + this.gameObject.name);
        }
    }

    void Update()
    {

        // 死亡している、またはプレイヤーが見つからない場合は何もしない
        if (isDead || playerTransform == null || agent == null) return;

        // プレイヤーと自分との距離を計算
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 1. プレイヤーが索敵範囲内にいるか？
        if (distanceToPlayer <= detectionRange)
        {
            // プレイヤーを追いかける
            agent.SetDestination(playerTransform.position);

            // 2. プレイヤーが攻撃範囲内に入り、かつ攻撃可能状態か？
            if (distanceToPlayer <= attackRange && canAttack)
            {
                // ここで攻撃処理を呼び出す
                AttackPlayer();
            }

        }
        else
        {
            // 索敵範囲外に出たら、追跡をやめる
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }
            // NavMeshAgentの現在の速度が0より大きいかどうかをAnimatorに伝える
            // agent.velocity.magnitude は現在の移動速度
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        
    }
    void AttackPlayer()
    {
        if (playerStats != null)
        {
            // プレイヤーの方を向く（任意ですが、より自然になります）
            transform.LookAt(playerTransform);

            // プレイヤーにダメージを与える
            playerStats.TakeDamage(attackPower);
            Debug.Log(this.name + " の攻撃！ " + playerStats.name + " に " + attackPower + " のダメージ！");

             //★ 攻撃アニメーションを再生（トリガー名はAnimator Controllerに合わせてください）
             animator.SetTrigger("Attack");

            // 攻撃クールダウンを開始
            StartCoroutine(AttackCooldown());
        }
    }

    // ダメージを受ける処理（簡略版）
    public void TakeDamage(int damage)
    {

        // すでに死亡しているか、ダメージ演出中の場合は処理しない
        if (isDead || isDamage) return;

        // ダメージSEを再生
        audioSource.PlayOneShot(seDamage);

        // ★ 'Damage'トリガーをセットして、ダメージアニメーションを再生
        animator.SetTrigger("Damage");

        isDamage = true;
        enemyHP -= damage;

        Debug.Log(this.name + " が " + damage + " のダメージを受けた！ 残りHP: " + enemyHP);

        // 追跡を一時停止
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        // 念のため、複数のRendererに対応できるようにしておく
        Renderer[] renderers = Body.GetComponentsInChildren<Renderer>(true);

        if (renderers.Length > 0)
        {
            // 点滅処理
            for (int i = 0; i < 3; i++)
            {
                foreach (var r in renderers) r.enabled = false;
                yield return new WaitForSeconds(0.1f);
                foreach (var r in renderers) r.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
        }


        if (enemyHP <= 0)
        {
            // 死亡処理を開始
            isDead = true; // 死亡状態にする（重要）
            audioSource.PlayOneShot(seDeath);
            // ★ 'Dead'をtrueにして、死亡アニメーションに遷移させる
            animator.SetBool("Dead", true);

            // 死亡アニメーションや演出が終わるのを待ってから処理を進める
            StartCoroutine(DieSequence());
            yield break;
        }
            isDamage = false;

            // 追跡を再開
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
            }
        
    }
        IEnumerator DieSequence()
        {
            // ナビゲーションを停止
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }

            // 死亡アニメーションが終わるのを少し待つ（時間はアニメーションに合わせて調整）
            yield return new WaitForSeconds(2.0f);

            //  プレイヤーに経験値を渡す
            if (playerStats != null && monsterData != null)
            {
                playerStats.GainExperience(monsterData.experiencePoint);
            }
        DropItems();

        //  モンスター自身を消滅させる
        Destroy(gameObject);
        }

    void DropItems()
    {

        foreach (var lootItem in monsterData.lootTable)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= lootItem.dropChance)
            {
                GameObject itemObject = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
                itemObject.GetComponent<DroppedItemController>().Initialize(lootItem.item);
                Debug.Log(lootItem.item.ItemName + " をドロップしました！");

                UIManager.instance.ShowMessage("あれは、、、");
            }
        }
    }

      
    

    // 攻撃のクールダウン処理
    IEnumerator AttackCooldown()
    {
        canAttack = false; // 攻撃不可状態にする
        yield return new WaitForSeconds(attackCooldown); // 指定秒数待機
        canAttack = true; // 攻撃可能状態に戻す
    }

    // ：索敵範囲と攻撃範囲をシーンビューで可視化する 
    private void OnDrawGizmosSelected()
    {
        // 索敵範囲を青いワイヤーフレームで表示
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 攻撃範囲を赤いワイヤーフレームで表示
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}



