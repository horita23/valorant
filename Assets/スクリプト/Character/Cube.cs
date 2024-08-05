using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class Skill_Info
{
    [Tooltip("The first skill of the character.")]
    public SkillBase skill;

    [Tooltip("Key to activate the first skill.")]
    public KeyCode skill_Key;
}

public enum StateSkill
{

    One = 0,
    Two = 1,
    COUNT = 2,

}
public class Cube : MonoBehaviourPunCallbacks
{
    [Tooltip("The name of the character.")]
    public string characterName;

    [Tooltip("The health points of the character.")]
    public float health = 100.0f;

    [Tooltip("The movement speed of the character.")]
    public float speed = 5f;

    [Tooltip("銃の種類")]
    public GameObject GunPrefab;//弾

    [Tooltip("The second skill of the character.")]
    public Skill_Info[] m_Skill_Info;

    private StateSkill m_StateSkill;

    private Animator animator = null;
    // Direction constants
    // Direction constants
    private const int Idle = 0;
    private const int Forward = 1;
    private const int ForwardRight = 2;
    private const int Right = 3;
    private const int BackwardRight = 4;
    private const int Backward = 5;
    private const int BackwardLeft = 6;
    private const int Left = 7;
    private const int ForwardLeft = 8;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            m_StateSkill = StateSkill.COUNT;
            for (int i = 0; i < m_Skill_Info.Length; i++)
            {
                m_Skill_Info[i].skill.ResetSkill();
            }

            animator = GetComponent<Animator>();

            PhotonNetwork.LocalPlayer.TagObject = this;
            var gunInstance = Instantiate(GunPrefab, transform.position, transform.rotation);
            gunInstance.transform.SetParent(this.transform); // プレイヤーの子要素に設定

        }
        PhotonNetwork.SendRate = 20; // 1秒間に20回送信
        PhotonNetwork.SerializationRate = 20; // 1秒間に20回シリアライズ
                                              // 銃のインスタンスを生成

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();
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

        // Normalize the input to ensure consistent movement speed
        if (input != Vector3.zero)
        {
            input.Normalize();
        }

        transform.Translate(speed * Time.deltaTime * input);

        if (input == Vector3.forward)
        {
            animator.SetInteger("Direction", Forward);
        }
        else if (input == (Vector3.forward + Vector3.right).normalized)
        {
            animator.SetInteger("Direction", ForwardRight);
        }
        else if (input == Vector3.right)
        {
            animator.SetInteger("Direction", Right);
        }
        else if (input == (Vector3.right + Vector3.back).normalized)
        {
            animator.SetInteger("Direction", BackwardRight);
        }
        else if (input == Vector3.back)
        {
            animator.SetInteger("Direction", Backward);
        }
        else if (input == (Vector3.back + Vector3.left).normalized)
        {
            animator.SetInteger("Direction", BackwardLeft);
        }
        else if (input == Vector3.left)
        {
            animator.SetInteger("Direction", Left);
        }
        else if (input == (Vector3.left + Vector3.forward).normalized)
        {
            animator.SetInteger("Direction", ForwardLeft);
        }
        else
        {
            animator.SetInteger("Direction", Idle);
        }
        Debug.Log(animator.GetInteger("Direction"));

        for (int i = 0; i < m_Skill_Info.Length; i++)
        {
            m_Skill_Info[i].skill.MUpdate(this);
            if (Input.GetKeyDown(m_Skill_Info[i].skill_Key))
            {
                m_Skill_Info[i].skill.Activate(this);
                m_StateSkill = (StateSkill)i;
            }
            
        }
        if ((int)m_StateSkill < m_Skill_Info.Length)
            m_Skill_Info[(int)m_StateSkill].skill.StateUpdate(this);

    }
}
