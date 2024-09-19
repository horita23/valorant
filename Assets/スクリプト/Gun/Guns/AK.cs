using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK : BaseGun
{
    [Tooltip("現在の弾薬")]
    private int currentAmmo;     // 現在の弾薬
    [Tooltip("集団率横")]
    public float horizontalSpread = 1.0f;
    [Tooltip("集団率縦")]
    public float verticalSpread = 1.0f; // M_rand, v
    public float probabilityFactor = 1.0f; // P^b

    public float recoilControlAmount = 0.5f; // リコイル制御量 (0.0 から 1.0)

    private int Recoil_Bullet_Count = 0;
    private float time = 0.0f;
    private Vector3 recoilOffset = Vector3.zero;
    private bool flag = false;

    //弾痕
    public GameObject bulletHolePrefab;

   
    public GameObject muzzleFlashParticle = null;
    public GameObject muzzleFlashPosiotn = null;

    RaycastHit hit;
    [SerializeField]
    LayerMask hitLayers = 0;

    void Start()
    {
        currentAmmo = ammoCapacity;
        Camera = FindObjectOfType<FastPersonCamera>();
 
    }

    public override void MainUpdate()
    {

    }
    public override void StateUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
        }
        else
        {
            flag = true;

        }
        if (currentAmmo <= 0)
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
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.parent.rotation, Time.deltaTime * 5);


            // 元の位置に十分近づいたら補間を停止する
            if (Quaternion.Angle(transform.rotation, transform.parent.rotation) < 0.01f)
            {
                transform.rotation = transform.parent.rotation;
                Recoil_Bullet_Count = 0;
                flag = false;
            }

        }
        if (Camera != null)
        {
            Camera.transform.rotation *= transform.rotation;
        }



    }

    public override void Shoot()
    {
        if (currentAmmo <= 0) return;

        time += Time.deltaTime;
        
        if (time > shotInterval)
        {
            time = 0.0f;
            currentAmmo--;
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
        currentAmmo = ammoCapacity;
    }

    public override void Recoil()
    {
        Vector2 recoilOffset = GenerateRandomPoint();
        Recoil_Bullet_Count++;

        // 縦の反動の上限を設定
        if (Recoil_Bullet_Count > Recoil_Bullet_limit)
        {
            // リコイルを適用
            transform.Rotate(new Vector3(0, recoilOffset.x, 0));

        }
        else
        {
            // リコイルを適用
            transform.Rotate(new Vector3(-Mathf.Abs(recoilOffset.y), recoilOffset.x, 0));

        }



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
