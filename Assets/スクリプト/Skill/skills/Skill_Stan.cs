using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill_Stan", menuName = "Skills/Skill_Stan")]
public class Skill_Stan : SkillBase
{
    protected override void UpdateSkill(Cube character)
    {
        character.transform.position=character.transform.position;
    }
    protected override void UseSkill(Cube character)
    {
        Debug.Log("Skill_Stan�X�L�������B");
        // �G���A�t���b�V���̋�̓I�Ȏ���
        Instantiate(SkillModel, character.transform.position, character.transform.rotation);



    }
}
