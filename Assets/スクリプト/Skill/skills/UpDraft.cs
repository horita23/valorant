using Photon.Pun;
using UnityEngine;
using static FlashSkill;

[CreateAssetMenu(fileName = "UpDraft", menuName = "Skills/UpDraft")]
public class UpDraft : SkillBase
{
    public float BRINKU_MOVE_TIME = 0.3f;
    private float brinkuTime = 0;
    private GameObject currentEffect;


    private float timer = 0f;

    protected override void Initialize(Cube character)
    {
    }

    protected override void UpdateSkill(Cube character)
    {

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;

        //起動時間の経過
        brinkuTime += Time.deltaTime;


        if (character.burinkSkillFlag)
        {
            timer += Time.deltaTime; // フレーム間の時間を加算
            if (timer >= BRINKU_MOVE_TIME)
            {
                EndBrinku();
                character.burinkSkillFlag = false;
                character.rb.velocity= new Vector3(character.rb.velocity.x, 0, character.rb.velocity.z);
            }
        }
        else
        {

            if (currentEffect != null)
                // エフェクトがキャラクターと一緒に移動
                currentEffect.transform.position = character.transform.position;
        }

        if (Input.GetKeyDown(GetSkill_Key))
        {
            // エフェクトのインスタンスを生成し、キャラクターの子オブジェクトにする
            currentEffect = Instantiate(SkillModel[0], character.transform.position, character.transform.rotation);
            currentEffect.transform.SetParent(character.transform);

            character.rb.AddForce(Vector3.up * 600);
            character.burinkSkillFlag = true;

        }


    }

    protected override void UseSkill(Cube character)
    {

    }

    protected override void ResetSkill(Cube character)
    {
    }

    private void EndBrinku()
    {
        brinkuTime = 0;
        timer = 0f; // タイマーをリセット
        LastUsedTimeSet();

        // エフェクトの削除
        if (currentEffect != null)
        {
            Destroy(currentEffect);
            currentEffect = null;
        }
    }
}
