using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkill : SkillBase
{
    private GameObject FlashModel;
    private Camera cam;  // ターゲットを表示するカメラ
    public GameObject FlashImgPrefab; // プレハブをインスペクターで設定
    private Image flashImg;

    private bool isVisible = false;

    private PhotonView targetView;

    private bool[] isHit = new bool[2];

    public enum Flash
    {
        NONE = 0,
        PREPARATION = 1,
        HOLD = 2,
        FLY_NOW = 3,
        FLY_END = 4,
    }

    Flash m_flash = Flash.NONE;

    protected override void Initialize(Cube character)
    {
        // ローカルプレイヤーオブジェクトを取得する
        var localPlayer = PhotonNetwork.LocalPlayer;
        int viewID = (int)localPlayer.CustomProperties[$"viewID_{localPlayer.ActorNumber}"];
        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;


        targetView = playerObject.GetComponent<PhotonView>();
        isHit[0] = false; // 初期化
        isHit[1] = false; // 初期化
        foreach (var player in PhotonNetwork.PlayerList)
        {

            // プレイヤーのカスタムプロパティを更新してフラグをセット
            player.CustomProperties[$"isFlashed{player.ActorNumber}"] = isHit;

            PhotonNetwork.SetPlayerCustomProperties(player.CustomProperties);
        }
        //// まずScene内のCanvasを探します
        //Canvas canvas = FindObjectOfType<Canvas>();

        //// FlashImgPrefabをCanvasの子要素として生成します
        //GameObject flashImgObject = Instantiate(FlashImgPrefab, canvas.transform);


        //// インスタンスからImageコンポーネントを取得
        //flashImg = flashImgObject.GetComponent<Image>();

        //flashImg.color = Color.clear;

        m_flash = Flash.NONE;
        cam = Camera.main;
    }

    protected override void UpdateSkill(Cube character)
    {
        switch (m_flash)
        {
            case Flash.NONE:
                break;
            case Flash.PREPARATION:


                //ネットワークで銃を作成する
                FlashModel = PhotonNetwork.Instantiate(SkillModel[0].name, character.transform.position + character.transform.forward * 1f + new Vector3(0, 1.5f, 0), character.transform.rotation);
                //プレイヤーを子にする
                targetView.RPC("SetParentRPC", RpcTarget.AllBuffered, FlashModel.GetPhotonView().ViewID, targetView.ViewID);

                //FlashModel = Instantiate(SkillModel[0], character.transform.position + character.transform.forward * 1f + new Vector3(0,1.5f,0), character.transform.rotation);
                //FlashModel.transform.SetParent(character.transform);
                FlashModel.GetComponent<Rigidbody>().isKinematic = true;

                m_flash = Flash.HOLD;
                break;
            case Flash.HOLD:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    m_flash = Flash.FLY_NOW;
                }

                break;
            case Flash.FLY_NOW:
                // プレイヤーの子要素から外す
                targetView.RPC("UnsetParentRPC", RpcTarget.AllBuffered, FlashModel.GetPhotonView().ViewID);
                FlashModel.GetComponent<Rigidbody>().isKinematic = false;

                // 初期速度を設定
                Rigidbody rb = FlashModel.GetComponent<Rigidbody>();
                rb.velocity = FlashModel.transform.forward * 5f;


                // 左クリックが押されているか確認
                if (Input.GetMouseButton(0))
                {
                    // マウスの移動量を取得
                    float horizontal = Input.GetAxis("Mouse X");
                    float vertical = Input.GetAxis("Mouse Y");

                    // 回転を計算
                    Vector3 rotation = new Vector3(-vertical, horizontal, 0) * 3f;

                    // オブジェクトを回転させる
                    FlashModel.transform.Rotate(rotation);
                }
                if (Input.GetKeyDown(GetSkill_Key))
                {
                    m_flash = Flash.FLY_END;
                }


                break;
            case Flash.FLY_END:
                // ターゲットのバウンディングボックスを考慮してカメラの視界内に少しでも入っているか確認
                Renderer targetRenderer = FlashModel.GetComponent<Renderer>();


                foreach (var player in PhotonNetwork.PlayerList)
                {
                    // ターゲットがカメラに映っているかの判定
                    Bounds bounds = targetRenderer.bounds;
                    Vector3[] corners = new Vector3[8];

                   
                    int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                    GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                    // プレイヤーのカメラを取得
                    Camera playerCam = playerObject.GetComponentInChildren<Camera>();

                    corners[0] = playerCam.WorldToViewportPoint(bounds.min);
                    corners[1] = playerCam.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
                    corners[2] = playerCam.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
                    corners[3] = playerCam.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
                    corners[4] = playerCam.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
                    corners[5] = playerCam.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
                    corners[6] = playerCam.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
                    corners[7] = playerCam.WorldToViewportPoint(bounds.max);

                    isVisible = false;
                    isHit[0] = false; // 初期化
                    foreach (Vector3 corner in corners)
                    {
                        // ビューポート内の0〜1の範囲にあれば視界内とする
                        if (corner.x >= 0.0f && corner.x <= 1.0f && corner.y >= 0.0f && corner.y <= 1.0f && corner.z > 0.0f)
                        {
                            isVisible = true;
                            break;
                        }
                    }
                    Vector3 directionToTarget;
                    directionToTarget = playerObject.transform.position - FlashModel.transform.position;
                    Debug.DrawRay(FlashModel.transform.position, directionToTarget * 10, Color.red, 10);

                    if (isVisible)
                    {
                        if (playerObject != null)
                        {



                            if (Physics.Raycast(FlashModel.transform.position, directionToTarget, out RaycastHit hitinfo, Mathf.Infinity))
                            {
                                if (hitinfo.collider.gameObject.CompareTag("Player"))
                                {
                                    //flashImg.color = new Color(0, 1, 0, 1);
                                    isHit[0] = true;
                                    Debug.Log($"{player.NickName}({player.ActorNumber})に表示されています");

                                }
                                else
                                {
                                    isHit[0] = false;

                                    Debug.Log($"{player.NickName}({player.ActorNumber})に表示されていません");
                                }


                            }
                        }


                        // UIの表示処理などを行う
                    }
                    else
                    {
                        if (Physics.Raycast(FlashModel.transform.position, directionToTarget, out RaycastHit hitinfo, Mathf.Infinity))
                        {
                            if (hitinfo.collider.gameObject.CompareTag("Player"))
                            {

                                isHit[1] = true;
                                Debug.Log($"{player.NickName}({player.ActorNumber})に表示されていません");

                            }
                            else
                            {
                                isHit[1] = false;

                                Debug.Log($"{player.NickName}({player.ActorNumber})に表示されていません");
                            }


                        }

                    }


                    // プレイヤーのカスタムプロパティを更新してフラグをセット
                    player.CustomProperties[$"isFlashed{player.ActorNumber}"] = isHit;

                    PhotonNetwork.SetPlayerCustomProperties(player.CustomProperties);



                }
                PhotonNetwork.Destroy(FlashModel);
                Destroy(FlashModel);
                m_flash = Flash.NONE;


                break;

            default:
                break;
        }

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;


        if (m_flash == Flash.NONE && Input.GetKeyDown(GetSkill_Key))
        {
            m_flash = Flash.PREPARATION;
        }

        if (m_flash == Flash.NONE)
        {
            isHit[0] = false; // 初期化
            isHit[1] = false; // 初期化
            foreach (var player in PhotonNetwork.PlayerList)
            {

                // プレイヤーのカスタムプロパティを更新してフラグをセット
                player.CustomProperties[$"isFlashed{player.ActorNumber}"] = isHit;

                PhotonNetwork.SetPlayerCustomProperties(player.CustomProperties);
            }
        }


        //if(!isVisible)
        //{
        //    flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime);
        //}
        //else
        //{
        //    flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime * 0.5f);

        //}

    }

    protected override void UseSkill(Cube character)
    {

    }


    private void EndBrinku()
    {
    }
}
