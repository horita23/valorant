using UnityEngine;
using System.Collections.Generic;

public interface ISkill
{
    void Activate();
    float Cooldown { get; }
    bool IsAvailable { get; }
    GameObject SkillModel { get; }
}

public abstract class SkillBase : ScriptableObject, ISkill
{
    public float cooldown;
    public GameObject skillModel;
    private float lastUsedTime;

    public float Cooldown => cooldown;

    public GameObject SkillModel => skillModel;

    public bool IsAvailable => (Time.time - lastUsedTime) >= cooldown;

    public void Activate()
    {
        if (IsAvailable)
        {
            UseSkill();
            lastUsedTime = Time.time;
        }
        else
        {
            Debug.Log("Skill is on cooldown.");
        }
    }

    protected abstract void UseSkill();
}
