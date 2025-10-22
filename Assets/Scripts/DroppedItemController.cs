
using System.Collections;
using UnityEngine;

public class DroppedItemController : MonoBehaviour
{
    public ItemData itemData; // このドロップ品が何のアイテムかの情報

    public AudioSource audioSource;

    public AudioClip Get;

    // ★追加: 拾われたかどうかを判定するフラグ
    private bool isPickedUp = false;


    // 他のスクリプトからこのアイテムの情報を設定するためのメソッド
    public void Initialize(ItemData data)
    {
        itemData = data;
        //GetComponent<SpriteRenderer>().sprite = data.itemIcon;
    }

    // 他のColliderがこのオブジェクトのTriggerに侵入した時に呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        // 侵入してきたのがプレイヤーかどうかをタグで判定
        if (other.CompareTag("Player") && !isPickedUp)
        {
            // すぐにフラグを立てて、他のプレイヤーや複数回の接触で何度も呼ばれるのを防ぐ
            isPickedUp = true;
            // ★ここにプレイヤーのインベントリにアイテムを追加する処理を書く★
            Debug.Log(itemData.ItemName + " を拾った！");

            // 音を再生し、その後にオブジェクトを消すコルーチンを開始する
            StartCoroutine(PickupSequence());
        }
    }
    private IEnumerator PickupSequence()
    {
        // まず音を再生する
        // 音源が設定されているかチェックすると、より安全
        if (audioSource != null && Get != null)
        {
            audioSource.PlayOneShot(Get);
        }

        // ★重要: ここで指定した秒数だけ、この関数の実行を一時停止する
        // 音が鳴り終わるのに十分な時間を待つ（例: 1秒）
        yield return new WaitForSeconds(1.0f);

        // 待機時間が終わったら、オブジェクトを消滅させる
        Destroy(gameObject);
    }
}