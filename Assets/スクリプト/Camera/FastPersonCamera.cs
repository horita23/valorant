using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    public GameObject target; //追従するターゲットオブジェクト
    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    private Cube playerAvatar; // プレイヤーのAvatarオブジェクト


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        playerAvatar = localPlayer.TagObject as Cube;
        if (playerAvatar != null && playerAvatar.photonView.IsMine)
        {
            // カメラの位置をターゲットの位置に追従
            transform.position = playerAvatar.transform.position;
            transform.Translate(new Vector3(0, 1, 0));

            // マウス入力によるカメラの回転
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            verticalRotation -= mouseY * MouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            horizontalRotation += mouseX * MouseSensitivity;

            // プレイヤーのAvatarオブジェクトをカメラの水平回転に合わせて回転
            playerAvatar.transform.rotation = Quaternion.Euler(0, horizontalRotation, 0);
            //カメラの回転
            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
        }

    }
}
