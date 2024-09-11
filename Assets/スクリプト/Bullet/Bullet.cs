using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����ȊO�̃v���C���[�ɓ��������ꍇ
        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine)
        {
            // �q�b�g�����v���C���[��PhotonView���擾
            PhotonView targetView = other.GetComponent<PhotonView>();

            if (targetView != null)
            {
                // �q�b�g�����v���C���[��RPC�Ń_���[�W�𑗂�
                targetView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }

            // �e�������Ȃǂ̏���
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

    }

}
