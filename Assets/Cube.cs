using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviourPunCallbacks, IPunObservable
{
    private const float InterpolationPeriod = 0.1f; // ��Ԃɂ����鎞��

    private Vector3 p1;
    private Vector3 p2;
    private Vector3 v1;
    private Vector3 v2;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.TagObject = this;
        }

        p1 = transform.position;
        p2 = p1;
        v1 = Vector3.zero;
        v2 = v1;
        elapsedTime = Time.deltaTime;

        PhotonNetwork.SendRate = 20; // 1�b�Ԃ�20�񑗐M
        PhotonNetwork.SerializationRate = 20; // 1�b�Ԃ�20��V���A���C�Y

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();
        }
        else
        {
            HandleInterpolation();
        }

    }

    private void HandleInput()
    {
        var input = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
            input += new Vector3(-1, 0, 0);

        if (Input.GetKey(KeyCode.D))
            input += new Vector3(1, 0, 0);

        if (Input.GetKey(KeyCode.W))
            input += new Vector3(0, 0, 1);

        if (Input.GetKey(KeyCode.S))
            input += new Vector3(0, 0, -1);


        // var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (input.sqrMagnitude > 0.01f)
        {
            p1 = p2;
            transform.Translate(6f * Time.deltaTime * input.normalized);
            p2 = transform.position;
            elapsedTime = 0f; // �ړ����������ꍇ�Ɍo�ߎ��Ԃ����Z�b�g
        }
        else
        {
            // ���͂��Ȃ��ꍇ�ł����݈ʒu���Ԃ̏I���ʒu�ɐݒ�
            p1 = p2;
            p2 = transform.position;
            v1 = v2 = Vector3.zero;
        }
    }

    private void HandleInterpolation()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < InterpolationPeriod)
        {
            transform.position = HermiteSpline.Interpolate(p1, p2, v1, v2, elapsedTime / InterpolationPeriod);
        }
        else
        {
            transform.position = p2;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            // ���t���[���̈ړ��ʂƌo�ߎ��Ԃ���A�b�������߂đ��M����
            stream.SendNext((p2 - p1) / elapsedTime);
        }
        else
        {
            var networkPosition = (Vector3)stream.ReceiveNext();
            var networkVelocity = (Vector3)stream.ReceiveNext();
            var lag = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) / 1000f);

            // ��M���̍��W���A��Ԃ̊J�n���W�ɂ���
            p1 = transform.position;
            // ���ݎ����ɂ�����\�����W���A��Ԃ̏I�����W�ɂ���
            p2 = networkPosition + networkVelocity * lag;
            // �O��̕�Ԃ̏I�����x���A��Ԃ̊J�n���x�ɂ���
            v1 = v2;
            // ��M�����b�����A��Ԃɂ����鎞�Ԃ�����̑��x�ɕϊ����āA��Ԃ̏I�����x�ɂ���
            v2 = networkVelocity * InterpolationPeriod;
            // �o�ߎ��Ԃ����Z�b�g����
            elapsedTime = 0f;
        }
    }

}

public static class HermiteSpline
{
    public static float Interpolate(float p1, float p2, float v1, float v2, float t)
    {
        float a = 2f * p1 - 2f * p2 + v1 + v2;
        float b = -3f * p1 + 3f * p2 - 2f * v1 - v2;
        return t * (t * (t * a + b) + v1) + p1;
    }

    public static Vector2 Interpolate(Vector2 p1, Vector2 p2, Vector2 v1, Vector2 v2, float t)
    {
        return new Vector2(
            Interpolate(p1.x, p2.x, v1.x, v2.x, t),
            Interpolate(p1.y, p2.y, v1.y, v2.y, t)
        );
    }

    public static Vector3 Interpolate(Vector3 p1, Vector3 p2, Vector3 v1, Vector3 v2, float t)
    {
        return new Vector3(
            Interpolate(p1.x, p2.x, v1.x, v2.x, t),
            Interpolate(p1.y, p2.y, v1.y, v2.y, t),
            Interpolate(p1.z, p2.z, v1.z, v2.z, t)
        );
    }
}

