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

    // 新しい抽象メソッドを定義
    protected abstract void UpdateSkill(Cube character);

    protected abstract void UseSkill(Cube character);

    // リセットメソッドを追加
    public void ResetSkill()
    {
        lastUsedTime = 0; // 冷却時間を考慮してリセット
    }
}
