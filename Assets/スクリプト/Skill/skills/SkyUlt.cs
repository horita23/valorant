using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "SkyUlt", menuName = "Skills/SkyUlt")]
public class SkyUlt : SkillBase
{
    public float MAX_BRINKU_TIME = 5;
    private float brinkuTime = 0;
    private GameObject currentEffect;

    private NavMeshAgent agent;

    public enum SkyUltState
    {
        NONE = 0,
        PREPARATION = 1,
        HOLD = 2,
        FLY_NOW = 3,
        FLY_END = 4,
    }
    SkyUltState m_skyUlt = SkyUltState.NONE;

    protected override void Initialize(Cube character) 
    {
        m_skyUlt = SkyUltState.NONE;
        agent = GetComponent<NavMeshAgent>();

    }

    protected override void UpdateSkill(Cube character)
    {

    }

    protected override void UpdateMein(Cube character)
    {
        if (!IsAvailable)
            return;

        switch (m_skyUlt)
        {
            case SkyUltState.NONE:
                break;
            case SkyUltState.PREPARATION:

                break;
            case SkyUltState.HOLD:
                break;
            case SkyUltState.FLY_NOW:
                foreach (var player in PhotonNetwork.PlayerListOthers)
                {
                    if (player.CustomProperties.TryGetValue($"viewID_{player.ActorNumber}", out object viewIDObj) && viewIDObj != null)
                    {
                        int viewID = (int)player.CustomProperties[$"viewID_{player.ActorNumber}"];
                        GameObject playerObject = PhotonView.Find(viewID)?.gameObject;

                        agent.destination = playerObject.transform.position;
                    }

                }
                break;
            case SkyUltState.FLY_END:
                break;

            default:
                break;
        }

        if (Input.GetKeyDown(GetSkill_Key))
        {
        }


    }

    protected override void UseSkill(Cube character)
    {

    }


    private void EndBrinku()
    {
    }
}
