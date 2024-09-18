using UnityEngine;

[CreateAssetMenu(fileName = "UpDraft", menuName = "Skills/UpDraft")]
public class UpDraft : SkillBase
{
    public float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private GameObject currentEffect;

    public enum BRINC
    {
        NONE = 0,
        Boot = 1,
    }

    BRINC m_BRINC = BRINC.NONE;

    protected override void Initialize(Cube character)
    {
        m_BRINC = BRINC.NONE;

    }

    protected override void UpdateSkill(Cube character)
    {

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;

        switch (m_BRINC)
        {
            case BRINC.NONE:
                break;
            case BRINC.Boot:
                //�N�����Ԃ̌o��
                brinkuTime += Time.deltaTime;

                if (currentEffect != null)
                    // �G�t�F�N�g���L�����N�^�[�ƈꏏ�Ɉړ�
                    currentEffect.transform.position = character.transform.position;

                if (brinkuTime >= MAX_BRINKU_TIME)
                    EndBrinku();
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(character.m_Skill_Info[0].skill_Key))
        {
            switch (m_BRINC)
            {
                case BRINC.NONE:
                    // �G�t�F�N�g�̃C���X�^���X�𐶐����A�L�����N�^�[�̎q�I�u�W�F�N�g�ɂ���
                    currentEffect = Instantiate(SkillModel[0], character.transform.position, character.transform.rotation);
                    currentEffect.transform.SetParent(character.transform);
                    m_BRINC=BRINC.Boot;
                    break;
                case BRINC.Boot:
                    character.transform.position += character.transform.forward * 5;
                    EndBrinku();
                    break;
                default:
                    break;
            }
        }


    }

    protected override void UseSkill(Cube character)
    {
        
    }


    private void EndBrinku()
    {
        m_BRINC=BRINC.NONE;
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
