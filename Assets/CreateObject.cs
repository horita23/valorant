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
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // �`�[���I�������擾
        string selectedTeam = PlayerPrefs.GetString("SelectedTeam");

        // �`�[���ɉ����ăX�|�[���ʒu������
        if (selectedTeam == "TeamA")
        {
            PhotonNetwork.Instantiate("Cube", new Vector3(0,3,0), Quaternion.identity);
        }
        else if (selectedTeam == "TeamB")
        {
            PhotonNetwork.Instantiate("Cube", new Vector3(5, 3, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("�`�[�����I������Ă��܂���");
        }
    }

}
