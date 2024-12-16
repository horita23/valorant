using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK : BaseGun
{
    [Tooltip("集団率横")]
    public float horizontalSpread = 1.0f;
    [Tooltip("集団率縦")]
    public float verticalSpread = 1.0f; // M_rand, v
    public float probabilityFactor = 1.0f; // P^b
    public float recoilControlAmount = 0.5f; // リコイル制御量 (0.0 から 1.0)
    public int magazin = 25;
    private int MaxammoCapacity = 0;
    public int Recoil_Bullet_Count { get; private set; }
    private float time = 0.0f;
    private Vector3 recoilOffset = Vector3.zero;
    public bool shotflag { get; private set; }

    //弾痕
    public GameObject bulletHolePrefab;

   
    public GameObject muzzleFlashParticle = null;
    public GameObject muzzleFlashPosiotn = null;

    private Quaternion FastGunRotate;
    // インスペクターで調整可能なEuler角
    [SerializeField] 
    private Vector3 fastGunRotateEuler = Vector3.zero;

    public Vector2 CurrentRecoil;
    public Vector2 currentrecoil => CurrentRecoil;   // 読み取り専用のプロパティ

    private float lastrecilY;

    RaycastHit hit;
    [SerializeField]
    LayerMask hitLayers = 0;
    void Start()
    {
        magazin = 25;
        MaxammoCapacity = ammoCapacity;
        RestBullet = magazin;

        var localPlayer = PhotonNetwork.LocalPlayer;
        Cube playerAvatar = localPlayer.TagObject as Cube;

        Camera = playerAvatar.GetComponentInChildren<FastPersonCamera>();

        shotflag = false;
    }

    public override void MainUpdate()
    {

    }
    public override void StateUpdate()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        Cube playerAvatar = localPlayer.TagObject as Cube;

        // ガンの回転を取得
        Quaternion gunRotation = playerAvatar.Shoulder[2].rotation;

        // z軸をリセットしたい場合、Euler角を利用
        Vector3 euler = gunRotation.eulerAngles;
        euler.z = 0; // z軸をリセット
        gunRotation = Quaternion.Euler(euler);

        // FastGunRotateの調整 (ここで微調整を加える)
        FastGunRotate = Quaternion.Euler(0,-13,0);

        transform.rotation = gunRotation * FastGunRotate;


        shotflag = false;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();

        }
        else
        {
            Recoil_Bullet_Count = 0;
            CurrentRecoil = Vector2.zero; // リコイルなし
            lastrecilY = 0.0f;

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }





    }

    public override void Shoot()
    {
        if (RestBullet <= 0) return;

        time += Time.deltaTime;
        var localPlayer = PhotonNetwork.LocalPlayer;
        Cube playerAvatar = localPlayer.TagObject as Cube;


        if (time > shotInterval)
        {
            time = 0.0f;
            RestBullet--;
            ammoCapacity--;
            shotflag = true;
            var flash = Instantiate(muzzleFlashParticle, muzzleFlashPosiotn.transform);
            

            if (Camera != null)
            {
                Debug.DrawRay(Camera.transform.position, Camera.transform.forward * 100, Color.red, 5);
                if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, 100.0f, hitLayers, QueryTriggerInteraction.Ignore))
                {
                    // 自分以外のプレイヤーに当たった場合
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        // ヒットしたプレイヤーのPhotonViewを取得
                        PhotonView targetView = hit.collider.GetComponent<PhotonView>();


                        if (targetView != null)
                        {

                            // ヒットしたプレイヤーにRPCでダメージを送る
                            targetView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);



                        }

                    }
                    else
                    {
                        // 弾痕を生成
                        GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        Destroy(bulletHole, 10.0f);
                    }


                    Debug.Log(hit.collider.gameObject.name);

                }
            }
            Recoil();


        }
    }

    public override void Reload()
    {
        if(ammoCapacity > 0)
            if(magazin < ammoCapacity)
                RestBullet = magazin;
            else
                RestBullet = ammoCapacity;

    }

    public override void Recoil()
    {
        Vector2 recoilOffset = GenerateRandomPoint();

        Recoil_Bullet_Count++;

        //// 縦の反動の上限を設定
        //if (Recoil_Bullet_Count > Recoil_Bullet_limit)
        //{
        //    // リコイルを適用
        //    CurrentRecoil = new Vector2(lastrecilY, recoilOffset.x);
        //}
        //else 
        //{
        //    // リコイルを適用
        //    CurrentRecoil = new Vector2(-Mathf.Abs(recoilOffset.y), recoilOffset.x); // カメラに渡すデータ
        //    lastrecilY = CurrentRecoil.y;
        //}

        if (Recoil_Bullet_Count < Recoil_Bullet_limit)
        {
            CurrentRecoil = RecoilPattern[Recoil_Bullet_Count];
        }
        else // 一定以降はランダムとか
        {
            CurrentRecoil = new Vector2(RecoilPattern[Recoil_Bullet_limit].x, Random.Range(-1.0f,1.5f));
        }


    }
    public override void RespawnReset()
    {
        ammoCapacity = MaxammoCapacity;
        RestBullet = magazin;
    }

    private void RecoilControl()
    {
        // リコイル効果を制御量と感度に基づいて減少させる
        recoilOffset = GenerateRandomPoint();
        transform.Rotate(new Vector3(-Mathf.Abs(recoilOffset.y) * (1 - recoilControlAmount), recoilOffset.x * (1 - recoilControlAmount), 0));
    }

    public Vector2 GenerateRandomPoint()
    {
        // スプレッドのためのランダムな角度を生成
        float angle = Random.Range(0f, Mathf.PI * 2);

        // ?q_rand を計算
        float deltaQRand = horizontalSpread * probabilityFactor * Mathf.Cos(angle);

        // 方程式を使用して ?r_rand を計算
        float deltaRRand = verticalSpread * Mathf.Sin(angle) * Mathf.Sqrt(1 - Mathf.Pow(deltaQRand / horizontalSpread, 2));

        // ランダムな点を返す
        return new Vector2(deltaQRand, deltaRRand);
    }
}
