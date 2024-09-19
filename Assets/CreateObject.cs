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
        Application.targetFrameRate = 200;
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

        GameObject playerObject = null;

        // �`�[���ɉ����ăX�|�[���ʒu�����肵�ăv���C���[�𐶐�
        if (selectedTeam == "TeamA")
        {
            playerObject = PhotonNetwork.Instantiate("Cube", new Vector3(0, 3, 0), Quaternion.identity);
        }
        else if (selectedTeam == "TeamB")
        {
            playerObject = PhotonNetwork.Instantiate("Cube", new Vector3(5, 3, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("�`�[�����I������Ă��܂���");
            return;
        }

        // �v���C���[��������PhotonView��ViewID��ۑ�
        PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
        int viewID = playerPhotonView.ViewID;

        // �J�X�^���v���p�e�B��ViewID��ۑ�
        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties["viewID"] = viewID;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

    }

}
