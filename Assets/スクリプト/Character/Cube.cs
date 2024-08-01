using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Tooltip("The second skill of the character.")]
    public Skill_Info[] m_Skill_Info;

    private StateSkill m_StateSkill;
    // Start is called before the first frame update
    void Start()
    {
        m_StateSkill = StateSkill.COUNT;
        for (int i = 0; i < m_Skill_Info.Length; i++)
        {
            m_Skill_Info[i].skill.ResetSkill();
        }
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
        transform.Translate(speed * Time.deltaTime * input.normalized);
    }
}
