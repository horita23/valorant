using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CreateObject1 : MonoBehaviourPunCallbacks
{
    public Transform[] respawnPositon;

    void Start()
    {
        Application.targetFrameRate = 200;
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;

        // マスターサーバーに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom("MatchRoom", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        AssignTeamAndSpawnPlayer();
    }

    private void AssignTeamAndSpawnPlayer()
    {
        GameObject playerObject;
        Cube.Team assignedTeam;
        Transform spawnPosition;

        // プレイヤー数に基づき交互にチームを割り当て
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount % 2 == 1)
        {
            assignedTeam = Cube.Team.TeamA;
            spawnPosition = respawnPositon[0];
        }
        else
        {
            assignedTeam = Cube.Team.TeamB;
            spawnPosition = respawnPositon[1];
        }

        // プレイヤーを生成し、チームを設定
        playerObject = PhotonNetwork.Instantiate("Cube", spawnPosition.position, Quaternion.identity);
        playerObject.GetComponent<Cube>().team = assignedTeam;
        playerObject.GetComponent<Cube>().respawnPositon = spawnPosition.position;

        // ViewIDの保存
        PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
        int viewID = playerPhotonView.ViewID;

        // カスタムプロパティにViewIDを保存
        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties[$"viewID_{PhotonNetwork.LocalPlayer.ActorNumber}"] = viewID;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
}

