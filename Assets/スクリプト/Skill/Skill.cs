using UnityEngine;
using System.Collections.Generic;

public interface ISkill 
{
    void Activate(Cube character);
    void MUpdate(Cube character);
    float Cooldown { get; }
    bool IsAvailable { get; }
    GameObject SkillModel { get; }
}

public abstract class SkillBase : ScriptableObject,ISkill
{
    public float cooldown;
    public GameObject skillModel;
    private float lastUsedTime;

    public float Cooldown => cooldown;

    public bool IsAvailable => (Time.time - lastUsedTime) >= cooldown;

    public GameObject SkillModel => skillModel;

    public void Activate(Cube character)
    {
        Debug.Log(lastUsedTime);
        if (IsAvailable)
        {
            UseSkill(character);
            lastUsedTime = Time.time;
        }
        else
        {
            Debug.Log("Skill is on cooldown.");
        }
    }
    public void MUpdate(Cube character)
    {
        UpdateSkill(character);
    }

    // �V�������ۃ��\�b�h���`
    protected abstract void UpdateSkill(Cube character);

    protected abstract void UseSkill(Cube character);

    // ���Z�b�g���\�b�h��ǉ�
    public void ResetSkill()
    {
        lastUsedTime = 0; // ��p���Ԃ��l�����ă��Z�b�g
    }
}
