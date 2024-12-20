using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkyUlt;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamDeathMatch : MonoBehaviour
{
    public float COOL_TIME_END = 2f;
    private float coolTimeEnd;

    public Text texxt;

    public Text messageText;       // 表示するテキスト
    public float displayDuration = 2f;   // テキストの表示時間
    public float fadeDuration = 0.5f;    // フェードイン・フェードアウトの時間

    private float CoolTimeEnd = 4.0f; // クールタイムを2秒に設定
    private bool isCoolTime = false; // クールタイムの状態を管理

    void EndMatch()
    {
        // 試合が終了したらクールタイム開始
        isCoolTime = true;
        CoolTimeEnd = Time.time + CoolTimeEnd; // 現在の時刻に2秒を足して終了時間を設定
    }
    public void ShowText(string message)
    {
        // メッセージをセットし、フェードイン開始
        messageText.text = message;
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // フェードイン
        messageText.CrossFadeAlpha(1f, fadeDuration, false);
        yield return new WaitForSeconds(displayDuration);

        // フェードアウト
        messageText.CrossFadeAlpha(0f, fadeDuration, false);
    }

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
        // 初期状態でテキストを非表示に
        messageText.canvasRenderer.SetAlpha(0f);

        coolTimeEnd = COOL_TIME_END;
        m_phase = Phase.PREPARATIONPHASE;
        GameTime = 0.0f;

        killCount[0] = 0;
        killCount[1] = 0;
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
                int killcount = (int)player.CustomProperties[$"killCount_{player.ActorNumber}"];

                
                //チームの合計キル数カウント
                if((Cube.Team)player.CustomProperties[$"Teme_{player.ActorNumber}"] == Cube.Team.TeamA)
                {
                    killCount[0] += killcount;

                }
                else
                {
                    killCount[1] += killcount;

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
                    if (killCount[i] == 10)
                    {
                        m_phase = Phase.ENDPHASE;
                        EndMatch();

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

                //coolTimeEnd -= Time.deltaTime;

                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
                    {
                        int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;


                        // カスタムプロパティにViewIDを保存
                        ExitGames.Client.Photon.Hashtable customProperties = player.CustomProperties;
                        // プレイヤーのActorNumberをキーにしてViewIDを保存
                        customProperties[$"GameEneFlag_{player.ActorNumber}"] = true;
                        player.SetCustomProperties(customProperties);

                    }

                }

               ShowText("終了!");

                Debug.Log(CoolTimeEnd);

                //if (coolTimeEnd<=0)
                //{
                //    if (PhotonNetwork.InRoom)
                //    {
                //        PhotonNetwork.LeaveRoom();
                //    }
                //    PhotonNetwork.Disconnect();

                //    SceneManager.LoadScene("TeamSelectScene");

                //}


                if (isCoolTime)
                {
                    // 現在の時刻がcoolTimeEndを超えるとクールタイム終了
                    if (Time.time >= CoolTimeEnd)
                    {
                        if (PhotonNetwork.InRoom)
                        {
                            PhotonNetwork.LocalPlayer.CustomProperties.Clear();

                            PhotonNetwork.LeaveRoom();
                        }
                        PhotonNetwork.Disconnect();

                        SceneManager.LoadScene("TeamSelectScene");

                        m_phase = Phase.PREPARATIONPHASE;

                    }
                }
                break;

            default:
                break;
        }


    }
}
