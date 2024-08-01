using UnityEngine;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkillBase : SkillBase
{
    public const float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private bool brinkuSurvivalFlag = false;
    protected override void UpdateSkill(Cube character)
    {

        //部リンク開始したか
        if (brinkuSurvivalFlag)
        {
            if(Input.GetKeyDown(character.m_Skill_Info[0].skill_Key))
                character.transform.position += character.transform.forward*5;
            brinkuTime += Time.deltaTime;
        }

        //時間オーバー
        if(brinkuTime >= MAX_BRINKU_TIME)
            brinkuSurvivalFlag = false;
    }
    protected override void UseSkill(Cube character)
    {
        Debug.Log("エリアフラッシュスキル発動。");

        Debug.Log("キャラクターのスピードが100に設定されました: " + character.speed);
        if (brinkuSurvivalFlag)
        {
            brinkuSurvivalFlag = false;
            brinkuTime = 0;
        }
        else
        {
            // エリアフラッシュの具体的な実装
            Instantiate(SkillModel, character.transform.position, character.transform.rotation);
            brinkuSurvivalFlag = true;
        }



    }
}
