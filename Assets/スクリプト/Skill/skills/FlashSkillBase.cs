using UnityEngine;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkillBase : SkillBase
{
    protected override void UseSkill(Cube character)
    {
        Debug.Log("�G���A�t���b�V���X�L�������B");
        // �G���A�t���b�V���̋�̓I�Ȏ���
        Instantiate(SkillModel, character.transform.position, character.transform.rotation);

    }
}
