using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    public GameObject target; //�Ǐ]����^�[�Q�b�g�I�u�W�F�N�g
    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    private Cube playerAvatar; // �v���C���[��Avatar�I�u�W�F�N�g


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
            // �J�����̈ʒu���^�[�Q�b�g�̈ʒu�ɒǏ]
            transform.position = playerAvatar.transform.position;
            transform.Translate(new Vector3(0, 1, 0));

            // �}�E�X���͂ɂ��J�����̉�]
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            verticalRotation -= mouseY * MouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            horizontalRotation += mouseX * MouseSensitivity;

            // �v���C���[��Avatar�I�u�W�F�N�g���J�����̐�����]�ɍ��킹�ĉ�]
            playerAvatar.transform.rotation = Quaternion.Euler(0, horizontalRotation, 0);
            //�J�����̉�]
            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
        }

    }
}
