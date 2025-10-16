using System.Collections;
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
    private AudioSource audioSource;
    public AudioClip seDamage;
    public GameObject Body; // 点滅させるメッシュを持つGameObject
    private bool isInvincible = false;


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
        }

        // このゲームオブジェクトに付いている AudioSource コンポーネントを取得して、
        // audioSource 変数に代入する
        audioSource = GetComponent<AudioSource>();

    }

    // ダメージを受ける処理（簡略版）
    public void TakeDamage(int damage)
    {

        // すでに死亡しているか、ダメージ演出中の場合は処理しない
        if (isDead || isDamage) return;

        // ダメージSEを再生
        audioSource.PlayOneShot(seDamage);

        isDamage = true;
        enemyHP -= damage;

        Debug.Log(this.name + " が " + damage + " のダメージを受けた！ 残りHP: " + enemyHP);

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

            Die();
            yield break; // Dieの中でDestroyするので、このコルーチンはここで終わり

        }
        isDamage = false;
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
                // ここでは簡略化のため、コンソールに表示するだけ
                Debug.Log(lootItem.item.itemName + " をドロップしました！");
                // TODO: 実際にアイテムを生成してプレイヤーが拾えるようにする
            }
        }
    }

}



