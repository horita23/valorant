using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using static FlashSkill;

[CreateAssetMenu(fileName = "SkyUlt", menuName = "Skills/SkyUlt")]
public class SkyUlt : SkillBase
{
    public float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    //スキルを使用する前のモデル
    private GameObject SkyUltPreparationModel;
    //スキル使用中のモデル
    private GameObject SkyUltModel;

    private PhotonView targetView;

    public enum SkyUltState
    {
        NONE = 0,
        PREPARATION = 1,//準備
        HOLD = 2,//待ち状態
        FLY_NOW = 3,//使用中
        FLY_END = 4,//使用後
    }
    SkyUltState m_skyUlt = SkyUltState.NONE;

    protected override void Initialize(Cube character) 
    {

        m_skyUlt = SkyUltState.NONE;

        // ローカルプレイヤーオブジェクトを取得する
        var localPlayer = PhotonNetwork.LocalPlayer;
        int viewID = (int)localPlayer.CustomProperties[$"viewID_{localPlayer.ActorNumber}"];
        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;


        targetView = playerObject.GetComponent<PhotonView>();


    }

    protected override void UpdateSkill(Cube character)
    {

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;

        switch (m_skyUlt)
        {
            case SkyUltState.NONE:

                if (Input.GetKeyDown(GetSkill_Key))
                {
                    m_skyUlt = SkyUltState.PREPARATION;
                }

                break;
            case SkyUltState.PREPARATION:

                //ネットワークで銃を作成する
                SkyUltPreparationModel = PhotonNetwork.Instantiate(SkillModel[0].name, character.transform.position + character.transform.forward * 1f + new Vector3(0, 1.5f, 0), character.transform.rotation);
                //プレイヤーを子にする
                targetView.RPC("SetParentRPC", RpcTarget.AllBuffered, SkyUltPreparationModel.GetPhotonView().ViewID, targetView.ViewID);

                SkyUltPreparationModel.GetComponent<Rigidbody>().isKinematic = true;


                m_skyUlt = SkyUltState.HOLD;

                break;
            case SkyUltState.HOLD:

                // 左クリックが押されているか確認
                if (Input.GetMouseButton(0))
                {
                    //スキルを使用する前のモデルを消す
                    PhotonNetwork.Destroy(SkyUltPreparationModel);
                    Destroy(SkyUltPreparationModel);


                    //ネットワークでスキル使用中モデルを作成する
                    SkyUltModel = PhotonNetwork.Instantiate(SkillModel[1].name, character.transform.position + character.transform.forward * 1f, character.transform.rotation);

                    //
                    //
                    m_skyUlt = SkyUltState.FLY_NOW;
                }
                    break;
            case SkyUltState.FLY_NOW:
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
                    {
                        int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                        SkyUltModel.GetComponent<NavMeshAgent>().destination = playerObject.transform.position;
                        float distance = Vector3.Distance(SkyUltModel.transform.position, playerObject.transform.position);
                        if (distance < 1.0f) // 例えば 1 メートル以内で衝突とみなす
                        {
                            m_skyUlt = SkyUltState.FLY_END;
                            // 衝突した時の処理を書く
                        }

                    }

                }
                break;
            case SkyUltState.FLY_END:
                //スキルを使用する前のモデルを消す
                PhotonNetwork.Destroy(SkyUltModel);
                Destroy(SkyUltModel);

                m_skyUlt = SkyUltState.NONE;

                break;

            default:
                break;
        }

        if (Input.GetKeyDown(GetSkill_Key))
        {
        }


    }

    protected override void UseSkill(Cube character)
    {

    }


    private void EndBrinku()
    {
    }
}
