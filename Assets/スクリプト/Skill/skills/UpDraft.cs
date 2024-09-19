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
                //起動時間の経過
                brinkuTime += Time.deltaTime;

                if (currentEffect != null)
                    // エフェクトがキャラクターと一緒に移動
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
                    // エフェクトのインスタンスを生成し、キャラクターの子オブジェクトにする
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

        // エフェクトの削除
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
    }
}
