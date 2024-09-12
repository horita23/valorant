using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

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
            // �}�E�X���͂ɂ��J�����̉�]
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            verticalRotation -= mouseY * MouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            horizontalRotation += mouseX * MouseSensitivity;

            // �v���C���[��Avatar�I�u�W�F�N�g���J�����̐�����]�ɍ��킹�ĉ�]
            Quaternion Rotation = Quaternion.Euler(0, horizontalRotation, 0);
            playerAvatar.transform.rotation = Rotation;

            //// ���݂̌��̉�]�ɁA�V������]��������
            //Quaternion currentRotation = playerAvatar.Shoulder[0].transform.rotation;
            //Quaternion currentRotation1 = playerAvatar.Shoulder[1].transform.rotation;

            //Quaternion additionalRotation = Quaternion.Euler(verticalRotation, 0, 0);
            //// ��]��K�p
            //playerAvatar.Shoulder[0].transform.rotation = currentRotation * additionalRotation;
            //playerAvatar.Shoulder[1].transform.rotation = currentRotation1 * additionalRotation;

            // Get the current rotation from the player's head and apply the rotation offset
            Quaternion targetRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            transform.rotation = targetRotation;

            // Set the camera's position relative to the player's head position, applying the offset in local space
            transform.position = playerAvatar.transform.position + Rotation * positionOffset;


        }



    }
}
