using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkyUlt;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class TeamDeathMatch : MonoBehaviour
{

    //フェーズ
    public enum Phase
    {
        NONE = 0,
        PREPARATIONPHASE = 1,//準備フェーズ
        FASTPHASE = 2,
        SECONDPHASE = 3,
        THIRDPHASE = 4,
        FORCEPHASE = 5,
        ENDPHASE = 6,
    }
    Phase m_phase = Phase.NONE;

    private float GameTime;

    private int[] killCount = new int[2];
    // Start is called before the first frame update
    void Start()
    {
        m_phase = Phase.NONE;
        GameTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム時間
        GameTime += Time.deltaTime;

        //すべてのプレイヤーの処理
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
            {
                int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                Cube playerScript = playerObject.GetComponent<Cube>();

                //チームの合計キル数カウント
                if(playerScript.GetTeam() == Cube.Team.TeamA)
                {
                    killCount[0] = playerScript.GetKillCount();

                }
                else
                {
                    killCount[1] = playerScript.GetKillCount();

                }
            }

        }

        switch (m_phase)
        {
            case Phase.NONE:

                break;
            case Phase.PREPARATIONPHASE://準備フェーズ
                //
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
                    {
                        int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;


                    }

                }

                //スポーンから出れないようにする、スキルは使えない

                //30秒たったら１フェーズに移行
                if (GameTime > 0.0f)
                {
                    m_phase = Phase.FASTPHASE;

                }

                break;
            case Phase.FASTPHASE:

                //1フェーズ目で使える選択されている武器を使う、スキルを使えるようにする、


                //どちらかのチームの合計キル数が20にだったら2フェーズ目に移行
                for(int i = 0; i < killCount.Length; i++) 
                {
                    if (killCount[i] == 5)
                    {
                        m_phase = Phase.ENDPHASE;

                    }
                    
                }
                break;
            case Phase.SECONDPHASE:
                break;
            case Phase.THIRDPHASE:
                
                break;
            case Phase.FORCEPHASE:
                
                break;

            case Phase.ENDPHASE:

                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
                    {
                        int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                        Cube playerScript = playerObject.GetComponent<Cube>();

                        playerScript.GameEndFlag = true;
                    }

                }

                break;

            default:
                break;
        }


    }
}
