using UnityEngine;

[CreateAssetMenu(fileName = "BlinkSkill", menuName = "Skills/BlinkSkill")]
public class BlinkSkill : SkillBase
{
    public float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private GameObject currentEffect;

    public enum Blink
    {
        NONE = 0,
        Boot = 1,
    }
    Blink m_blink = Blink.NONE;

    protected override void Initialize(Cube character) 
    {
        m_blink = Blink.NONE;
    }

    protected override void UpdateSkill(Cube character)
    {

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;

        switch (m_blink)
        {
            case Blink.NONE:
                break;
            case Blink.Boot:
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

        if (Input.GetKeyDown(GetSkill_Key))
        {
            switch (m_blink)
            {
                case Blink.NONE:
                    // �G�t�F�N�g�̃C���X�^���X�𐶐����A�L�����N�^�[�̎q�I�u�W�F�N�g�ɂ���
                    currentEffect = Instantiate(SkillModel[0], character.transform.position, character.transform.rotation);
                    currentEffect.transform.SetParent(character.transform);
                    m_blink=Blink.Boot;
                    break;
                case Blink.Boot:
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
        m_blink=Blink.NONE;
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
