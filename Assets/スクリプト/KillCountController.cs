using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCountController : MonoBehaviour
{
    public Text[] scoreboardPanel; // スコアボードのPanelをアタッチ

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int indext = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
            {
                int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                int killCount = (int)player.CustomProperties[$"killCount_{player.ActorNumber}"];

                scoreboardPanel[indext].text = killCount.ToString();
            
            }

            indext++;
        }

    }
}
