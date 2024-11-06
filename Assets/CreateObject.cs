using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObject : MonoBehaviourPunCallbacks
{
    public Transform[] respawnPositon;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 200;
        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = "Player";

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // チーム選択情報を取得
        string selectedTeam = PlayerPrefs.GetString("SelectedTeam");

        GameObject playerObject = null;

        // チームに応じてスポーン位置を決定してプレイヤーを生成
        if (selectedTeam == "TeamA")
        {
            playerObject = PhotonNetwork.Instantiate("Cube", respawnPositon[0].position, Quaternion.identity);

            playerObject.GetComponent<Cube>().team = Cube.Team.TeamA;
            playerObject.GetComponent<Cube>().respawnPositon = respawnPositon[0].position;
        }
        else if (selectedTeam == "TeamB")
        {
            playerObject = PhotonNetwork.Instantiate("Cube", respawnPositon[1].position, Quaternion.identity);
            playerObject.GetComponent<Cube>().respawnPositon = respawnPositon[1].position;
            playerObject.GetComponent<Cube>().team = Cube.Team.TeamB;

        }
        else
        {
            Debug.LogError("チームが選択されていません");
            return;
        }

        // プレイヤー生成時にPhotonViewのViewIDを保存
        PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
        int viewID = playerPhotonView.ViewID;



        // カスタムプロパティにViewIDを保存
        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        // プレイヤーのActorNumberをキーにしてViewIDを保存
        customProperties[$"viewID_{PhotonNetwork.LocalPlayer.ActorNumber}"] = viewID;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);


    }

}
