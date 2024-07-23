using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviourPunCallbacks
{
    public SkillBase skill1; // インスペクタから設定可能
    public SkillBase skill2; // インスペクタから設定可能
    public string characterName;
    public float health = 100.0f;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {

        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.TagObject = this;
        }
        PhotonNetwork.SendRate = 20; // 1秒間に20回送信
        PhotonNetwork.SerializationRate = 20; // 1秒間に20回シリアライズ
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
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

        if (Input.GetKeyDown(KeyCode.Q))
            skill1.Activate();

        if (Input.GetKeyDown(KeyCode.E))
            skill2.Activate();

        transform.Translate(6f * Time.deltaTime * input.normalized);
    }
}
