using UnityEngine;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkillBase : SkillBase
{
    protected override void UseSkill(Cube character)
    {
        Debug.Log("エリアフラッシュスキル発動。");
        // エリアフラッシュの具体的な実装
        Instantiate(SkillModel, character.transform.position, character.transform.rotation);

    }
}
