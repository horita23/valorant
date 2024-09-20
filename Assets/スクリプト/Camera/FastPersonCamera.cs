using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    private float verticalRotation;

    private Cube playerAvatar; // プレイヤーのAvatarオブジェクト

    public Vector3 positionOffset;  // Offset for camera position


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame

    private void LateUpdate()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        playerAvatar = localPlayer.TagObject as Cube;
        if (playerAvatar != null && playerAvatar.photonView.IsMine)
        {
            if (playerAvatar.gunInstance)
            {
                // マウス入力によるカメラの回転
                float mouseY = Input.GetAxis("Mouse Y");
                verticalRotation -= mouseY * playerAvatar.MouseSensitivity;

                //
                verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);


                // プレイヤーのAvatarオブジェクトをカメラの水平回転に合わせて回転
                transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
                transform.position = playerAvatar.CameraPosition.position;

            }
        }
    }
}
