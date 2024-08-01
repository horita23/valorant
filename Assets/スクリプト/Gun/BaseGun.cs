using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviourPunCallbacks
{
    [Tooltip("�e�̖��O")]
    public string gunName;  //�e�̖��O
    [Tooltip("�e�̎��")]
    public GameObject bulletPrefab;//�e
    [Tooltip("�e�̑��x")]
    public float shotSpeed;//���x
    [Tooltip("�_���[�W")]
    public float damage;      //�_���[�W
    [Tooltip("�e��e��")]
    public int ammoCapacity;    //�e��e��
    [Tooltip("�e���o��Ԋu")]
    public float shotInterval;//�e���o��Ԋu
    [Tooltip("�����A���Œe�𔭎˂����������ɒ��˂锽���̏����������e�̐�")]
    public int Recoil_Bullet_limit;

    public FastPersonCamera Camera;

    // target�t�B�[���h�ɃA�N�Z�X���邽�߂̃v���p�e�B
    //����
    public abstract void Shoot();
    //�����[�h
    public abstract void Reload();
    //���R�C��
    public abstract void Recoil();

}
