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

    // Start is called before the first frame update
    void Start()
    {
        FastCameraPositon = new Vector3(-0.119999997f, 0.0599999987f, 0.200000003f);
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
                // マウス入力によるカメラの回転
                float mouseY = Input.GetAxis("Mouse Y");

                verticalRotation -= mouseY * playerAvatar.MouseSensitivity;

                verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);


                // カメラの垂直回転を適用
                transform.localRotation = Quaternion.Euler(verticalRotation , 0, 0);

                if (playerAvatar.gunInstance != null)
                {
                    var GunRotate = playerAvatar.gunInstance.GetComponent<AK>();

                    // ガンの回転を取得
                    Quaternion gunRotation = GunRotate.GunTransform.localRotation;

                    // z軸の回転をリセット
                    gunRotation.z = 0;

                    // 新しい回転を適用
                    transform.localRotation *= gunRotation; 
                }


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
