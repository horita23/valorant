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
    private int Recoil_Bullet_Count = 0;
    private float time = 0.0f;
    private Vector3 recoilOffset = Vector3.zero;
    private bool flag = false;

    //弾痕
    public GameObject bulletHolePrefab;

   
    public GameObject muzzleFlashParticle = null;
    public GameObject muzzleFlashPosiotn = null;

    private Quaternion FastGunRotate;
    // インスペクターで調整可能なEuler角
    [SerializeField] 
    private Vector3 fastGunRotateEuler = Vector3.zero;

    public Vector2 CurrentRecoil { get; private set; }
    private Quaternion OriginalRotation; // 元の回転を保存

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

        

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }
        else
        {
            flag = true;

        }
        if (RestBullet <= 0)
        {
            flag = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (flag)
        {
            // 補間して元の位置に戻す
           // transform.rotation = Quaternion.Lerp(transform.rotation, gunRotation, Time.deltaTime * 5);


            // 元の位置に十分近づいたら補間を停止する
            if (Quaternion.Angle(transform.rotation, gunRotation) < 0.01f)
            {
                //transform.rotation = transform.parent.rotation;
                
            }
            Recoil_Bullet_Count = 0;
            CurrentRecoil = Vector2.zero; // リコイルなし

            flag = false;
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
        if (Recoil_Bullet_Count == 0)
        {
            OriginalRotation = transform.rotation;

        }

        Recoil_Bullet_Count++;

        // 縦の反動の上限を設定
        if (Recoil_Bullet_Count > Recoil_Bullet_limit)
        {
            // リコイルを適用
            transform.Rotate(new Vector3(0, recoilOffset.x, 0));
            CurrentRecoil = new Vector2(0, recoilOffset.x);
        }
        else if(Recoil_Bullet_Count > 2)
        {
            // リコイルを適用
            transform.Rotate(new Vector3(-Mathf.Abs(recoilOffset.y), recoilOffset.x, 0));
            CurrentRecoil = new Vector2(-Mathf.Abs(recoilOffset.y), recoilOffset.x); // カメラに渡すデータ
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
