using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Match : MonoBehaviourPunCallbacks
{
    public Text matchStatusText;      // マッチング状況表示テキスト
    public Button matchButton;        // マッチング開始/キャンセルボタン
    public string playSceneName = "LowPolyFPSLite/Scenes/LowPolyFPS_Lite_Demo"; // 遷移先シーン名

    private bool isMatched = false;   // マッチング完了フラグ
    private bool isMatching = false;  // マッチング中フラグ

    void Start()
    {
        // 初期設定
        Application.targetFrameRate = 60;
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;

        PhotonNetwork.ConnectUsingSettings();

        // UI初期化
        matchStatusText.text = "接続中...";
        matchButton.onClick.AddListener(OnMatchButtonClicked);
    }

    void OnMatchButtonClicked()
    {
        if (isMatching)
        {
            // マッチングキャンセル処理
            CancelMatching();
        }
        else
        {
            // マッチング開始処理
            StartMatching();
        }
    }

    void StartMatching()
    {
        if (!PhotonNetwork.IsConnected) return;

        matchStatusText.text = "マッチング中...";
        matchButton.GetComponentInChildren<Text>().text = "キャンセル"; // ボタンテキスト変更
        isMatching = true;

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom("MatchRoom", roomOptions, TypedLobby.Default);
        matchButton.interactable = true;
    }

    void CancelMatching()
    {
        // マッチングをキャンセルしてロビーに戻る
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        matchStatusText.text = "マッチングがキャンセルされました";
        matchButton.GetComponentInChildren<Text>().text = "マッチング開始"; // ボタンテキスト変更
        isMatching = false;
    }

    public override void OnConnectedToMaster()
    {
        matchStatusText.text = "ボタンを押してマッチングを開始";
        matchButton.GetComponentInChildren<Text>().text = "マッチング開始";
        matchButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        matchStatusText.text = "ルームに参加しました。プレイヤーを待っています...";
        CheckPlayersInRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CheckPlayersInRoom();
    }

    public override void OnLeftRoom()
    {
        // ルームを離れたときの処理
        matchStatusText.text = "マッチングがキャンセルされました";
        matchButton.GetComponentInChildren<Text>().text = "マッチング開始";
        isMatching = false;
    }

    private void CheckPlayersInRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !isMatched)
        {
            isMatched = true;
            StartCoroutine(StartGameAfterDelay());
        }
    }

    IEnumerator StartGameAfterDelay()
    {
        matchStatusText.text = "マッチングしました！ゲーム開始...";
        yield return new WaitForSeconds(3f);

        // PhotonNetwork.LoadLevelを使用してシーン遷移
        PhotonNetwork.LoadLevel(playSceneName);
    }
}
