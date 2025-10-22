using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCotroller : MonoBehaviour
{
    //回転設定
    public float mouseSensitivity = 2.0f; // マウス感度
    public float minVerticalAngle = -15.0f; // 下向き制限
    public float maxVerticalAngle = 15.0f;  // 上向き制限
                                            //追従設定
    Transform player; // プレイヤー参照
    public Vector3 offset = new Vector3(0f, 2f, -5f); // プレイヤーからの相対位置
    public float followSpeed = 16f; // 追従スピード
    float verticalRotation = 0f; // カメラの上下角
    float currentYaw = 0f;       // プレイヤーの左右回転（自由）
    private Vector3 rotation;

    // ★追加: カメラ操作を有効/無効にするためのフラグ
    public bool isCameraActive = true;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
            {
                enabled = false;
                return;
            }
        }

        // ★UIモードからプレイモードに戻った時にカーソルをロックする
        LockCursor();

        // カーソルロック
       // Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // 初期角度設定
        Vector3 angles = transform.eulerAngles;
        currentYaw = angles.y;
        verticalRotation = angles.x;
    }

    void Update()
    {
        // ★追加: isCameraActiveがtrueの時だけカメラ操作を実行する
        if (isCameraActive)
        {
            // マウス入力
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // 横回転（制限なし）
            currentYaw += mouseX;

            // 縦回転（制限あり）
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

            // カメラ回転を適用
            Quaternion rotation = Quaternion.Euler(verticalRotation, currentYaw, 0f);
            // プレイヤーの後方に位置するようにカメラ座標を計算
            Vector3 desiredPosition = player.position + rotation * offset;

            // スムーズに追従
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // カメラをプレイヤーに向ける
            transform.LookAt(player.position + Vector3.up * 2f);

            // プレイヤーの向きをカメラのY回転に合わせる（上下は無視）
            player.rotation = Quaternion.Euler(0f, currentYaw, 0f);
        }
    }
    // ★追加: カーソルをロック/表示するための関数を外から呼べるようにする
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCameraActive = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCameraActive = false;
    }

}
