using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastPersonCamera : MonoBehaviourPunCallbacks
{

    [SerializeField] private float _returnSpeed = 1f;      // 補間速度
    [SerializeField] private float _snappiness = 6f;      // リコイルのスナップ性
    private Vector3 _recoilTargetRotation;                // リコイル目標回転
    private Vector3 _currentRecoilRotation;               // 現在のリコイル回転
    private Vector3 _returnTarget;                        // 補間時の目標位置

    private bool isInterpolating = false;                 // 補間中フラグ
    private Cube playerAvatar;                            // プレイヤー情報
    private float verticalRotation = 0f;                  // 垂直回転
    private float interpolationSpeed = 5f;                // 補間速度設定
    private Quaternion hokancamera;                       // 回転保存用

    private Vector3 FastCameraPosition = new Vector3(-0.12f, 0.06f, 0.2f);

    private int recoilPatternIndex;

    void LateUpdate()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        playerAvatar = localPlayer.TagObject as Cube;

        if (playerAvatar != null && photonView.IsMine)
        {
            // 銃のリコイルを取得
            AK gun = playerAvatar.gunInstance?.GetComponent<AK>();
            if (gun != null)
            {
                // リコイルが発生しているとき
                if (gun.shotflag)
                {
                    isInterpolating = true;

                    Recoil(gun.currentrecoil);

                }
                else
                {
                    recoilPatternIndex = 0;

                }

                // 銃を撃っておらず、リコイル中かつ補間が必要な場合
                if (!gun.shotflag && isInterpolating)
                {
                    hokancamera = transform.localRotation;
                    _returnTarget = Vector3.zero; // リコイルの目標回転をリセット
                }
            }

            // 垂直回転をマウス入力に基づいて更新
            float mouseY = Input.GetAxis("Mouse Y");
            verticalRotation -= mouseY * playerAvatar.MouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            // リコイルの反映処理
            ReflectsRecoil();

            // 回転の適用（カメラと体の回転）
            transform.localRotation = Quaternion.Euler(verticalRotation + _currentRecoilRotation.x, _currentRecoilRotation.y, 0);

            // 補間処理の終了条件
            if (isInterpolating && Quaternion.Angle(transform.localRotation, hokancamera) < 0.01f)
            {
                isInterpolating = false; // 補間終了
            }

            Transform spineBone = playerAvatar.transform.Find("jett/TP_Wushu_S0_Skelmesh.ao/Skeleton/Root/Splitter/Spine1");
            if (spineBone != null)
            {
                spineBone.localRotation *= transform.localRotation;
            }

            Transform neckBone = playerAvatar.transform.Find("jett/TP_Wushu_S0_Skelmesh.ao/Skeleton/Root/Splitter/Spine1/Spine2/Spine3/Spine4/Neck");
            if (neckBone != null)
            {
                Vector3 rotatedPosition = neckBone.rotation * FastCameraPosition;
                transform.position = neckBone.position + rotatedPosition;
            }

        }

    }

    /// <summary>指定したリコイルを設定する</summary>
    public void Recoil(Vector2 recoil)
    {
        _recoilTargetRotation += new Vector3(recoil.x, recoil.y, 0);
    }

    /// <summary>リコイルを反映させる</summary>
    void ReflectsRecoil()
    {
        _recoilTargetRotation = Vector3.Slerp(_recoilTargetRotation, _returnTarget,
            _returnSpeed * Time.fixedDeltaTime);
        _currentRecoilRotation = Vector3.Slerp(_currentRecoilRotation, _recoilTargetRotation,
            _snappiness * Time.fixedDeltaTime);
    }

}
