using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CreateObject1 : MonoBehaviourPunCallbacks
{
    public Transform[] respawnPositon;
    public string playSceneName = "LowPolyFPSLite/Scenes/LowPolyFPS_Lite_Demo";

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
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // シーン遷移後に必ず実行されるようにする
        Debug.Log("Scene Loaded: " + scene.name);
            AssignTeamAndSpawnPlayer();
    }

    private void AssignTeamAndSpawnPlayer()
    {
        PhotonNetwork.NickName = "Player";

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

