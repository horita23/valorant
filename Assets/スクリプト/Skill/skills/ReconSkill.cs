using System.Linq;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "ReconSkill", menuName = "Skills/ReconSkill")]
public class ReconSkill : SkillBase
{
    private GameObject ReconModel;
    private Rigidbody Rigidbody;

    public enum Recon
    {
        NONE = 0,
        HOLD = 1,
        SHOT = 2,
        SEARCH = 3
    }
    Recon m_recon = Recon.NONE;

    protected override void Initialize(Cube character)
    {
        m_recon = Recon.NONE;

    }

    protected override void UpdateSkill(Cube character)
    {
        switch (m_recon)
        {
            case Recon.NONE:
                break;
            case Recon.HOLD:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    m_recon = Recon.SHOT;
                }

                break;
            case Recon.SHOT:
                ReconModel = Instantiate(SkillModel[0], new Vector3(10,3,0), character.transform.rotation);
                //Rigidbody = ReconModel.GetComponent<Rigidbody>();
                //Rigidbody.AddForce(character.transform.forward * 10); //キャラクターが向いている方向に弾に力を加える

                m_recon = Recon.SEARCH;

                break;
            default:
                break;
        }

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;


        if (Input.GetKeyDown(GetSkill_Key))
        {
            m_recon = Recon.HOLD;
        }

        if(m_recon == Recon.SEARCH)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                PerformRaycastCheck(ReconModel, character,100f);
            }

        }


    }
    // Recon.shotの状態でレイキャスト判定を行うメソッド
    private void PerformRaycastCheck(GameObject obj, Cube cube, float searchRange)
    {
        Collider[] hitColliders = Physics.OverlapSphere(obj.transform.position, searchRange); // 半径10の範囲内のオブジェクトを取得
        foreach (var collider in hitColliders)
        {
            var something = collider.gameObject;

            if (!something.CompareTag("Player")) { continue; } // タグがPlayer以外なら無視

            foreach (Transform point in cube.ReconColliderPositon)
            {
                Vector3 direction = point.position - ReconModel.transform.position;
                Debug.DrawRay(ReconModel.transform.position, direction * 10, Color.red, 10);
                if (!Physics.Raycast(ReconModel.transform.position, direction, out RaycastHit hitinfo, Mathf.Infinity))
                {
                    return;
                }
                if (hitinfo.collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("見ぃ〜つっけた！");
                    return;
                }
                else
                {
                    Debug.Log("見つけてない！");
                }

            }        //    // SkinnedMeshRendererを取得
                     //    SkinnedMeshRenderer skinnedMeshRenderer = something.GetComponentInChildren<SkinnedMeshRenderer>();
                     //    if (skinnedMeshRenderer == null) { continue; }

            //    // SkinnedMeshRendererからメッシュを取得
            //    Mesh skinnedMesh = new Mesh();
            //    skinnedMeshRenderer.BakeMesh(skinnedMesh); // SkinnedMeshをベイクして現在の頂点情報を取得

            //    // 頂点と法線を取得
            //    Vector3[] vertices = skinnedMesh.vertices;
            //    Vector3[] normals = skinnedMesh.normals;

            //    // キャラクターの中心を取得
            //    Vector3 characterCenter = skinnedMeshRenderer.transform.position;

            //    foreach (var (vertex, normal) in vertices.Zip(normals, Tuple.Create))
            //    {
            //        // キャラクターの外周にあるかを判定（法線が外向きの場合）
            //        if (Vector3.Dot(normal, (vertex - characterCenter).normalized) > 0.95f) // 外周かどうかの閾値（0.7など調整可能）
            //        {
            //            // ワールド座標に変換
            //            Vector3 worldVertex = something.transform.TransformPoint(vertex);
            //            Vector3 direction = worldVertex - ReconModel.transform.position; // ReconModelの位置から頂点への方向ベクトルを計算

            //            Debug.DrawRay(ReconModel.transform.position, direction * 10, Color.red, 30);

            //            if (Physics.Raycast(ReconModel.transform.position, direction, out RaycastHit hitinfo, Mathf.Infinity))
            //            {
            //                if (hitinfo.collider.gameObject.CompareTag("Player"))
            //                {
            //                    Debug.Log("見ぃ〜つっけた！");
            //                    return;
            //                }
            //                else
            //                {
            //                    Debug.Log("見つけてない！");
            //                }
            //            }
            //        }
            //    }
        }


    }

    // 衝突が発生した時の処理
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトの情報を取得
        GameObject hitObject = collision.gameObject;

        // 衝突後に弾を停止させる
        Rigidbody.velocity = Vector3.zero; // 速度をゼロにする
        Rigidbody.angularVelocity = Vector3.zero; // 回転を止める

        // 必要に応じて弾をその場で固定したい場合
        Rigidbody.isKinematic = true; // 物理演算を無効にして、位置が変わらないようにする

        // デバッグ用メッセージ
        Debug.Log("弾が " + hitObject.name + " に当たった！");
    }
    protected override void UseSkill(Cube character)
    {


    }


    private void EndBrinku()
    {
        m_recon = Recon.NONE;
        LastUsedTimeSet();

    }
}
