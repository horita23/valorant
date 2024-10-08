﻿using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkill : SkillBase
{
    private GameObject FlashModel;
    public Camera cam;  // ターゲットを表示するカメラ
    public GameObject FlashImgPrefab; // プレハブをインスペクターで設定
    private Image flashImg;

    private bool isVisible = false;

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

        // まずScene内のCanvasを探します
        Canvas canvas = FindObjectOfType<Canvas>();

        // FlashImgPrefabをCanvasの子要素として生成します
        GameObject flashImgObject = Instantiate(FlashImgPrefab, canvas.transform);


        // インスタンスからImageコンポーネントを取得
        flashImg = flashImgObject.GetComponent<Image>();

        flashImg.color = Color.clear;

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

                FlashModel = Instantiate(SkillModel[0], character.transform.position + character.transform.forward * 1f + new Vector3(0,1.5f,0), character.transform.rotation);
                FlashModel.transform.SetParent(character.transform);
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

                FlashModel.transform.SetParent(null);
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
                // ターゲットがカメラに映っているかの判定
                if (cam != null && FlashModel != null)
                {
                    Bounds bounds = targetRenderer.bounds;
                    Vector3[] corners = new Vector3[8];

                    corners[0] = cam.WorldToViewportPoint(bounds.min);
                    corners[1] = cam.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
                    corners[2] = cam.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
                    corners[3] = cam.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
                    corners[4] = cam.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
                    corners[5] = cam.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
                    corners[6] = cam.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
                    corners[7] = cam.WorldToViewportPoint(bounds.max);

                    isVisible = false;
                    foreach (Vector3 corner in corners)
                    {
                        // ビューポート内の0〜1の範囲にあれば視界内とする
                        if (corner.x >= 0.0f && corner.x <= 1.0f && corner.y >= 0.0f && corner.y <= 1.0f && corner.z > 0.0f)
                        {
                            isVisible = true;
                            break;
                        }
                    }
                    if (isVisible)
                    {

                        Vector3 directionToTarget = character.transform.position - FlashModel.transform.position;
                        Debug.DrawRay(FlashModel.transform.position, directionToTarget * 10, Color.red, 10);

                        if (Physics.Raycast(FlashModel.transform.position, directionToTarget, out RaycastHit hitinfo, Mathf.Infinity))
                        {
                            if (hitinfo.collider.gameObject.CompareTag("Player"))
                            {
                                flashImg.color = new Color(0, 1, 0, 1);

                                Debug.Log("ターゲットはカメラに表示されています");

                            }
                            else
                            {
                                Debug.Log("ターゲットはカメラに表示されていません");
                            }


                        }


                        // UIの表示処理などを行う
                    }
                    else
                    {
                        flashImg.color = new Color(1, 1, 1, 1);

                        Debug.Log("ターゲットはカメラに表示されていません");
                    }
                }
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

        if(!isVisible)
        {
            flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime);
        }
        else
        {
            flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime * 0.5f);

        }

    }


    protected override void UseSkill(Cube character)
    {

    }


    private void EndBrinku()
    {
    }
}
