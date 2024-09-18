using UnityEngine;

[CreateAssetMenu(fileName = "FlashSkill", menuName = "Skills/FlashSkill")]
public class FlashSkill : SkillBase
{
    private GameObject FlashModel;
    public GameObject FlashRender;

    public enum Flash
    {
        NONE = 0,
        PREPARATION = 1,
        HOLD = 2,
        FLY_NOW = 3,
        FLY_END = 4,
    }

    Flash m_flash = Flash.NONE;

    protected override void Initialize(Cube character)
    {
        m_flash = Flash.NONE;
    }

    protected override void UpdateSkill(Cube character)
    {
        switch (m_flash)
        {
            case Flash.NONE:
                break;
            case Flash.PREPARATION:
                FlashModel = Instantiate(SkillModel[0], character.transform.position + character.transform.forward * 1f + new Vector3(0,1.5f,0), character.transform.rotation);
                FlashModel.transform.SetParent(character.transform);
                FlashModel.GetComponent<Rigidbody>().isKinematic = true;

                m_flash = Flash.HOLD;
                break;
            case Flash.HOLD:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    m_flash = Flash.FLY_NOW;
                }

                break;
            case Flash.FLY_NOW:

                FlashModel.transform.SetParent(null);
                FlashModel.GetComponent<Rigidbody>().isKinematic = false;

                // 初期速度を設定
                Rigidbody rb = FlashModel.GetComponent<Rigidbody>();
                rb.velocity = FlashModel.transform.forward * 5f;


                // 左クリックが押されているか確認
                if (Input.GetMouseButton(0))
                {
                    // マウスの移動量を取得
                    float horizontal = Input.GetAxis("Mouse X");
                    float vertical = Input.GetAxis("Mouse Y");

                    // 回転を計算
                    Vector3 rotation = new Vector3(-vertical, horizontal, 0) * 3f;

                    // オブジェクトを回転させる
                    FlashModel.transform.Rotate(rotation);
                }
                if (Input.GetKeyDown(GetSkill_Key))
                {
                    m_flash = Flash.FLY_END;
                }


                break;
            case Flash.FLY_END:

                
                Destroy(FlashModel);
                m_flash = Flash.NONE;

                break;

            default:
                break;
        }

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;


        if (m_flash == Flash.NONE && Input.GetKeyDown(GetSkill_Key))
        {
            m_flash = Flash.PREPARATION;
        }


    }


    
    protected override void UseSkill(Cube character)
    {

    }


    private void EndBrinku()
    {
    }
}
