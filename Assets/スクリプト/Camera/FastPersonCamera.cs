using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    private float verticalRotation;

    private Cube playerAvatar; // �v���C���[��Avatar�I�u�W�F�N�g

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
                // �}�E�X���͂ɂ��J�����̉�]
                float mouseY = Input.GetAxis("Mouse Y");
                verticalRotation -= mouseY * playerAvatar.MouseSensitivity;

                //
                verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);


                // �v���C���[��Avatar�I�u�W�F�N�g���J�����̐�����]�ɍ��킹�ĉ�]
                transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
                transform.position = playerAvatar.CameraPosition.position;

            }
        }
    }
}
