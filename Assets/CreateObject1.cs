using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CreateObject1 : MonoBehaviourPunCallbacks
{
    public Transform[] respawnPositon;
    public string playSceneName = "LowPolyFPS_Lite_Demo";

    private void OnEnable()
    {
        // シーンロード時のコールバックを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // コールバックの解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // シーン遷移後に必ず実行されるようにする
        Debug.Log("Scene Loaded: " + scene.name);
        AssignTeamAndSpawnPlayer();
    }

    private void AssignTeamAndSpawnPlayer()
    {
        GameObject playerObject;
        Cube.Team assignedTeam;
        Transform spawnPosition;

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // ActorNumber を使ってリスポーン位置を決定
        int spawnIndex = actorNumber % respawnPositon.Length;
        spawnPosition = respawnPositon[spawnIndex];

        // チームを交互に割り当てる
        assignedTeam = (actorNumber % 2 == 1) ? Cube.Team.TeamA : Cube.Team.TeamB;


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

