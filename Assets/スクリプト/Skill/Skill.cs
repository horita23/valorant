using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public interface ISkill 
{
    void initialize(Cube character, KeyCode skill_key);
    void Activate(Cube character);
    void MUpdate(Cube character);
    void StateUpdate(Cube character);
    float Cooldown { get; }
    bool IsAvailable { get; }
    GameObject[] SkillModel { get; }
    KeyCode GetSkill_Key { get; }

}

public abstract class SkillBase : MonoBehaviourPunCallbacks, ISkill
{
    public float cooldown;
    public GameObject[] skillModel;
    private float lastUsedTime;

    public float Cooldown => cooldown;

    public bool IsAvailable => (Time.time - lastUsedTime) >= cooldown;

    public GameObject[] SkillModel => skillModel;

    private  KeyCode skill_Key;

    public KeyCode GetSkill_Key => skill_Key;

    public void initialize(Cube character,KeyCode skill_key)
    {
        skill_Key = skill_key;
        lastUsedTime = 0; // 冷却時間を考慮してリセット
        Initialize(character);
    }
    public void Activate(Cube character)
    {
        Debug.Log(lastUsedTime);
        if (IsAvailable)
        {
            UseSkill(character);
        }
        else
        {
            Debug.Log("Skill is on cooldown.");
        }
    }
    public void MUpdate(Cube character)
    {
        UpdateMein(character);
    }
    public void StateUpdate(Cube character)
    {
        UpdateSkill(character);
    }
    // 新しい抽象メソッドを定義
    protected abstract void UpdateSkill(Cube character);
    protected abstract void UpdateMein(Cube character);
    protected abstract void UseSkill(Cube character);
    protected abstract void Initialize(Cube character);    


    protected void LastUsedTimeSet()
    {
        lastUsedTime = Time.time;
    }

}
