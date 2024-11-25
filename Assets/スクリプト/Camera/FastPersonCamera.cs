using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    private float verticalRotation;

    private Cube playerAvatar; // プレイヤーのAvatarオブジェクト

    public Vector3 positionOffset;  // Offset for camera position

    private Vector3 FastCameraPositon;

    private Vector2 cameraRecoilOffset = Vector2.zero;

    private Quaternion hokancamera = Quaternion.identity;
    private bool isRecoiling = false; // 現在リコイルが適用されているか
    private bool isInterpolating = false; // リコイル補間中か
    private float interpolationSpeed = 5f; // 補間の速度

    // Start is called before the first frame update
    void Start()
    {
        FastCameraPositon = new Vector3(-0.119999997f, 0.0599999987f, 0.200000003f);
        isInterpolating = false;
        isRecoiling = false;
    }

    // Update is called once per frame

    private void LateUpdate()
    {

        var localPlayer = PhotonNetwork.LocalPlayer;
        playerAvatar = localPlayer.TagObject as Cube;

        if (playerAvatar != null && photonView.IsMine)
        {
            if (playerAvatar.gunInstance)
            {
                AK gun = playerAvatar.gunInstance.GetComponent<AK>();
                if (gun != null)
                {
                    //if (gun.CurrentRecoil == Vector2.zero)
                    //{
                    //    isRecoiling = false;

                    //    // リコイルがゼロのとき、補間を開始
                    //    if (!isInterpolating)
                    //    {
                    //        isInterpolating = true;
                    //        hokancamera = transform.rotation; // 現在のカメラ回転を保存
                    //    }
                    //}
                    //else
                    //{
                    //    // リコイルが発生中
                    //    isRecoiling = true;
                    //    cameraRecoilOffset = gun.CurrentRecoil;
                    //}
                    cameraRecoilOffset = gun.CurrentRecoil;
                }

                // 垂直回転にリコイルの縦方向のオフセットを追加
                verticalRotation += cameraRecoilOffset.x;

                // マウス入力によるカメラの回転
                float mouseY = Input.GetAxis("Mouse Y");

                verticalRotation -= mouseY * playerAvatar.MouseSensitivity;

                verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);


                // カメラの垂直回転を適用
                transform.localRotation = Quaternion.Euler(verticalRotation , 0, 0);

                if (playerAvatar.gunInstance != null)
                {

                    // ガンの回転を取得
                    Quaternion gunRotation = playerAvatar.gunInstance.transform.localRotation;

                    // z軸の回転をリセット
                    gunRotation.z = 0;

                    // 新しい回転を適用
                    // transform.localRotation *= gunRotation; 
                    transform.localRotation *= Quaternion.Euler(0, cameraRecoilOffset.y, 0);
                }
                // 補間処理
                //if (isInterpolating && !isRecoiling)
                //{
                //    // リコイルがない場合、補間で元の回転に戻る
                //    transform.rotation = Quaternion.Lerp(transform.rotation, hokancamera, Time.deltaTime * interpolationSpeed);

                //    // 補間が完了したら状態をリセット
                //    if (Quaternion.Angle(transform.rotation, hokancamera) < 0.01f)
                //    {
                //        transform.rotation = hokancamera;
                //        isInterpolating = false; // 補間終了
                //        cameraRecoilOffset = Vector2.zero; // リコイルなし
                //    }
                //}

                Transform spineBone = playerAvatar.transform.Find("jett/TP_Wushu_S0_Skelmesh.ao/Skeleton/Root/Splitter/Spine1"); // 上半身のボーン名に合わせて変更
                if (spineBone != null)
                {
                    // 上半身を少し回転させる（オプション）
                    spineBone.localRotation *= transform.localRotation;

                }
                Transform Splitter = playerAvatar.transform.Find("jett/TP_Wushu_S0_Skelmesh.ao/Skeleton/Root/Splitter/Spine1/Spine2/Spine3/Spine4/Neck"); // 上半身のボーン名に合わせて変更
                if (Splitter != null)
                {
                    Vector3 rotatedPosition = Splitter.rotation * FastCameraPositon;
                    transform.position = Splitter.transform.position + rotatedPosition;
                }

            }
        }
    }


}
