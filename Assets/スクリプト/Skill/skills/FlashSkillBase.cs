using UnityEngine;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkillBase : SkillBase
{
    public float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private bool brinkuSurvivalFlag = false;
    private bool brinkuBootFlag = false;
    private GameObject currentEffect;
    protected override void UpdateSkill(Cube character)
    {
    }

    protected override void UpdateMein(Cube character)
    {
        Debug.Log("�G���A�t���b�V���X�L�������B");
        
        if (brinkuBootFlag)
        {
            brinkuTime += Time.deltaTime;

            if (currentEffect != null)
                // �G�t�F�N�g���L�����N�^�[�ƈꏏ�Ɉړ�
                currentEffect.transform.position = character.transform.position;

            if (Input.GetKeyDown(character.m_Skill_Info[0].skill_Key))
            {
                character.transform.position += character.transform.forward * 5;
                EndBrinku();
            }

            if (brinkuTime >= MAX_BRINKU_TIME)
                EndBrinku();
        }

        //�������N�N��
        //�A���ŃL�[��������邱�Ƃ�����̂�2�d�`�F�b�N
        if (brinkuSurvivalFlag)
            brinkuBootFlag = true;

    }

    protected override void UseSkill(Cube character)
    {
        if (!brinkuSurvivalFlag)
        {
            // �G�t�F�N�g�̃C���X�^���X�𐶐����A�L�����N�^�[�̎q�I�u�W�F�N�g�ɂ���
            currentEffect = Instantiate(SkillModel[0], character.transform.position, character.transform.rotation);
            currentEffect.transform.SetParent(character.transform);

            //�������N�N��
            brinkuSurvivalFlag = true;
        }

    }


    private void EndBrinku()
    {
        brinkuBootFlag = false;
        brinkuSurvivalFlag = false;
        brinkuTime = 0;
        LastUsedTimeSet();

        // �G�t�F�N�g�̍폜
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
    }
}
