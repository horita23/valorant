using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviourPunCallbacks
{
    [Tooltip("銃の名前")]
    public string gunName;  //銃の名前
    [Tooltip("弾の種類")]
    public GameObject bulletPrefab;//弾
    [Tooltip("弾の速度")]
    public float shotSpeed;//速度
    [Tooltip("ダメージ")]
    public float damage;      //ダメージ
    [Tooltip("弾薬容量")]
    public int ammoCapacity;    //弾薬容量
    [Tooltip("弾が出る間隔")]
    public float shotInterval;//弾が出る間隔
    [Tooltip("何発連続で弾を発射したら上方向に跳ねる反動の上限をかける弾の数")]
    public int Recoil_Bullet_limit;

    public FastPersonCamera Camera;

    // targetフィールドにアクセスするためのプロパティ
    //発射
    public abstract void Shoot();
    //リロード
    public abstract void Reload();
    //リコイル
    public abstract void Recoil();

}
