using UnityEngine;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkillBase : SkillBase
{
    public const float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private bool brinkuSurvivalFlag = false;
    protected override void UpdateSkill(Cube character)
    {

        //�������N�J�n������
        if (brinkuSurvivalFlag)
        {
            if(Input.GetKeyDown(character.m_Skill_Info[0].skill_Key))
                character.transform.position += character.transform.forward*5;
            brinkuTime += Time.deltaTime;
        }

        //���ԃI�[�o�[
        if(brinkuTime >= MAX_BRINKU_TIME)
            brinkuSurvivalFlag = false;
    }
    protected override void UseSkill(Cube character)
    {
        Debug.Log("�G���A�t���b�V���X�L�������B");

        Debug.Log("�L�����N�^�[�̃X�s�[�h��100�ɐݒ肳��܂���: " + character.speed);
        if (brinkuSurvivalFlag)
        {
            brinkuSurvivalFlag = false;
            brinkuTime = 0;
        }
        else
        {
            // �G���A�t���b�V���̋�̓I�Ȏ���
            Instantiate(SkillModel, character.transform.position, character.transform.rotation);
            brinkuSurvivalFlag = true;
        }



    }
}
