using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK : BaseGun
{
    [Tooltip("���݂̒e��")]
    private int currentAmmo;     // ���݂̒e��
    [Tooltip("�W�c����")]
    public float horizontalSpread = 1.0f;
    [Tooltip("�W�c���c")]
    public float verticalSpread = 1.0f; // M_rand, v
    public float probabilityFactor = 1.0f; // P^b

    public float recoilControlAmount = 0.5f; // ���R�C������� (0.0 ���� 1.0)

    private int Recoil_Bullet_Count = 0;
    private float time = 0.0f;
    private Vector3 recoilOffset = Vector3.zero;
    private bool flag = false;

    //�e��
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
            // ��Ԃ��Č��̈ʒu�ɖ߂�
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.parent.rotation, Time.deltaTime * 5);


            // ���̈ʒu�ɏ\���߂Â������Ԃ��~����
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
                    // �����ȊO�̃v���C���[�ɓ��������ꍇ
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        // �q�b�g�����v���C���[��PhotonView���擾
                        PhotonView targetView = hit.collider.GetComponent<PhotonView>();

                        if (targetView != null)
                        {
                            // �q�b�g�����v���C���[��RPC�Ń_���[�W�𑗂�
                            targetView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                        }

                    }
                    else
                    {
                        // �e���𐶐�
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

        // �c�̔����̏����ݒ�
        if (Recoil_Bullet_Count > Recoil_Bullet_limit)
        {
            // ���R�C����K�p
            transform.Rotate(new Vector3(0, recoilOffset.x, 0));

        }
        else
        {
            // ���R�C����K�p
            transform.Rotate(new Vector3(-Mathf.Abs(recoilOffset.y), recoilOffset.x, 0));

        }



    }

    private void RecoilControl()
    {
        // ���R�C�����ʂ𐧌�ʂƊ��x�Ɋ�Â��Č���������
        recoilOffset = GenerateRandomPoint();
        transform.Rotate(new Vector3(-Mathf.Abs(recoilOffset.y) * (1 - recoilControlAmount), recoilOffset.x * (1 - recoilControlAmount), 0));
    }

    public Vector2 GenerateRandomPoint()
    {
        // �X�v���b�h�̂��߂̃����_���Ȋp�x�𐶐�
        float angle = Random.Range(0f, Mathf.PI * 2);

        // ?q_rand ���v�Z
        float deltaQRand = horizontalSpread * probabilityFactor * Mathf.Cos(angle);

        // ���������g�p���� ?r_rand ���v�Z
        float deltaRRand = verticalSpread * Mathf.Sin(angle) * Mathf.Sqrt(1 - Mathf.Pow(deltaQRand / horizontalSpread, 2));

        // �����_���ȓ_��Ԃ�
        return new Vector2(deltaQRand, deltaRRand);
    }
}
