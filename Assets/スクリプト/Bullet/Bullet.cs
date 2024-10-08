using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float time;

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 自分以外のプレイヤーに当たった場合
        if (other.gameObject.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine)
        {
            // ヒットしたプレイヤーのPhotonViewを取得
            PhotonView targetView = other.GetComponent<PhotonView>();

            if (targetView != null)
            {
                // ヒットしたプレイヤーにRPCでダメージを送る
                targetView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }

            // 弾を消すなどの処理
            Destroy(gameObject);
        }
    }
        // Start is called before the first frame update
     void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > 3.0f)
        {
            // 射撃後3秒で弾丸のオブジェクトを破壊する
            //PhotonNetwork.Destroy(gameObject);
            Destroy(gameObject);

            time = 0.0f;
        }

    }

}
