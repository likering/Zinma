using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f; // 最大HP
    private float currentHealth;   // 現在のHP

    void Start()
    {
        currentHealth = maxHealth;
    }

    // ダメージを受ける処理
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " は " + amount + " のダメージを受けた！ 残りHP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // 死亡処理
    void Die()
    {
        Debug.Log(gameObject.name + " を倒した！");
        Destroy(gameObject); // オブジェクトをシーンから削除
    }
}
