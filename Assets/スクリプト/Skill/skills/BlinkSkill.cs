using Photon.Pun;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static FlashSkill;

[CreateAssetMenu(fileName = "BlinkSkill", menuName = "Skills/BlinkSkill")]
public class BlinkSkill : SkillBase
{
    public float BRINKU_MOVE_TIME = 0.3f;
    public float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private GameObject currentEffect;

    private float timer = 0f;
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
                //起動時間の経過
                brinkuTime += Time.deltaTime;
                            

                if (character.burinkSkillFlag_2)
                {
                    timer += Time.deltaTime; // フレーム間の時間を加算
                    if (timer >= BRINKU_MOVE_TIME)
                    {
                        EndBrinku();
                        character.burinkSkillFlag = false;
                        character.burinkSkillFlag_2 = false;
                    }
                }
                else
                {

                    if (currentEffect != null)
                        // エフェクトがキャラクターと一緒に移動
                        currentEffect.transform.position = character.transform.position;

                    if (brinkuTime >= MAX_BRINKU_TIME)
                        EndBrinku();
                }
                break;
            default:
                break;
        }

        if (Input.GetKeyDown(GetSkill_Key))
        {
            switch (m_blink)
            {
                case Blink.NONE:
                    // エフェクトのインスタンスを生成し、キャラクターの子オブジェクトにする
                    currentEffect = Instantiate(SkillModel[0], character.transform.position, character.transform.rotation);
                    currentEffect.transform.SetParent(character.transform);
                    m_blink=Blink.Boot;
                    character.burinkSkillFlag_2 = false;
                    break;
                case Blink.Boot:
                    // プレイヤーの移動方向を取得（高さを無視）
                    Vector3 moveDirection = character.rb.velocity;
                    moveDirection.y = 0; // 高さを無視
                    character.rb.AddForce(moveDirection.normalized * 600);
                    character.burinkSkillFlag = true;
                    character.burinkSkillFlag_2 = true;

                    break;
                default:
                    break;
            }
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
        m_blink=Blink.NONE;
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
