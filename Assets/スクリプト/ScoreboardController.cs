using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    public GameObject scoreboardPanel; // スコアボードのPanelをアタッチ
    public GameObject PlayerScore;
    // Start is called before the first frame update
    void Start()
    {
        scoreboardPanel.SetActive(false); // 初期状態では非表示
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboardPanel.SetActive(true); // Tabキーを押すと表示
                                             //すべてのプレイヤーの処理
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
                {
                    int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                    GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                    Cube playerScript = playerObject.GetComponent<Cube>();

                }

            }

        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboardPanel.SetActive(false); // Tabキーを離すと非表示
        }
    }
}
