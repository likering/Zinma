using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyController : MonoBehaviour
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
        // 死亡していない、かつプレイヤーが存在する場合、プレイヤーを追跡する
        if (!isDead && agent != null && playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
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

            yield return new WaitForSeconds(3.0f);

            Die();
            yield break; // Dieの中でDestroyするので、このコルーチンはここで終わり

        }
        isDamage = false;

        // 追跡を再開
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }


    void Die()
    {
        // 1. プレイヤーに経験値を渡す
        if (playerStats != null && monsterData != null)
        {
            playerStats.GainExperience(monsterData.experiencePoint);
        }

        // 2. アイテムドロップ処理
        DropItems();

        // 3. モンスター自身を消滅させる
        Destroy(gameObject);
    }

    void DropItems()
    {
        // ドロップテーブル内の各アイテムについて、ドロップするかどうかを確率で判定
        foreach (var lootItem in monsterData.lootTable)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= lootItem.dropChance)
            {
                // ★ アイテムをシーン上に生成する（別途ドロップアイテム用のPrefabが必要）
                // プレハブから新しいGameObjectを生成
                GameObject itemObject = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);

                // 生成したオブジェクトのスクリプトを取得し、アイテム情報を設定する
                itemObject.GetComponent<DroppedItemController>().Initialize(lootItem.item);
                Debug.Log(lootItem.item.ItemName + " をドロップしました！");
                //  実際にアイテムを生成してプレイヤーが拾えるようにする
            }
        }
    }

    // プレイヤーとの衝突判定
    private void OnCollisionEnter(Collision collision)
    {
        // 攻撃可能状態で、衝突した相手がプレイヤーの場合
        if (canAttack && collision.gameObject.CompareTag("Player"))
        {
            if (playerStats != null)
            {
                // プレイヤーにダメージを与える
                playerStats.TakeDamage(attackPower);
                Debug.Log(this.name + " の攻撃！ " + playerStats.name + " に " + attackPower + " のダメージ！");

                // 攻撃クールダウンを開始
                StartCoroutine(AttackCooldown());
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

}



