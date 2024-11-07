using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FlashSkill;

[CreateAssetMenu(fileName = "Skill_Stan", menuName = "Skills/Skill_Stan")]
public class Skill_Stan : SkillBase
{
    protected override void Initialize(Cube character)
    {
    }
    protected override void UpdateMein(Cube character)
    {

    }

    protected override void UpdateSkill(Cube character)
    {

    }
    protected override void UseSkill(Cube character)
    {
        Debug.Log("Skill_StanÉXÉLÉãî≠ìÆÅB");
        character.transform.position += new Vector3(0,5,0);
        LastUsedTimeSet();
    }

    protected override void ResetSkill(Cube character)
    {
    }

}
