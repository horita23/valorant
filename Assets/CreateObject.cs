using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[���g�̖��O��"Player"�ɐݒ肷��
        PhotonNetwork.NickName = "Player";

        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        // �����_���ȍ��W�Ɏ��g�̃A�o�^�[�i�l�b�g���[�N�I�u�W�F�N�g�j�𐶐�����
        var position = new Vector3(0,-1,0);
        PhotonNetwork.Instantiate("Cube", position, Quaternion.identity);
    }

}
