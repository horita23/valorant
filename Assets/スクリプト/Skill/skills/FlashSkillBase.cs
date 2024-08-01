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
        Debug.Log("エリアフラッシュスキル発動。");
        
        if (brinkuBootFlag)
        {
            brinkuTime += Time.deltaTime;

            if (currentEffect != null)
                // エフェクトがキャラクターと一緒に移動
                currentEffect.transform.position = character.transform.position;

            if (Input.GetKeyDown(character.m_Skill_Info[0].skill_Key))
            {
                character.transform.position += character.transform.forward * 5;
                EndBrinku();
            }

            if (brinkuTime >= MAX_BRINKU_TIME)
                EndBrinku();
        }

        //部リンク起動
        //連続でキーが押されることがあるので2重チェック
        if (brinkuSurvivalFlag)
            brinkuBootFlag = true;

    }

    protected override void UseSkill(Cube character)
    {
        if (!brinkuSurvivalFlag)
        {
            // エフェクトのインスタンスを生成し、キャラクターの子オブジェクトにする
            currentEffect = Instantiate(SkillModel[0], character.transform.position, character.transform.rotation);
            currentEffect.transform.SetParent(character.transform);

            //部リンク起動
            brinkuSurvivalFlag = true;
        }

    }


    private void EndBrinku()
    {
        brinkuBootFlag = false;
        brinkuSurvivalFlag = false;
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
