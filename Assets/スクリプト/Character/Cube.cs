using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

[System.Serializable]
public class Skill_Info
{
    [Tooltip("The first skill of the character.")]
    public SkillBase skill;

    [Tooltip("Key to activate the first skill.")]
    public KeyCode skill_Key;
}

public enum StateSkill
{
    One = 0,
    Two = 1,
    COUNT = 2,
}

public class Cube : MonoBehaviourPunCallbacks
{
    [Tooltip("The name of the character.")]
    public string characterName;

    [Tooltip("The health points of the character.")]
    public float HEALTH = 100.0f;
    private float health = 0.0f;

    [Tooltip("The movement speed of the character.")]
    public float RunSpeed = 5f;
    public float WalkSpeedRatio = 0.8f;
    public float CrouchSpeedRatio = 0.6f;

    [Tooltip("銃の種類")]
    public GameObject GunPrefab;
    private GameObject gunInstance;

    [Tooltip("The second skill of the character.")]
    public Skill_Info[] m_Skill_Info;

    // Jump parameters
    public float jumpForce = 10f;

    private StateSkill m_StateSkill;

    private Animator animator = null;

    private Slider slider;

        //孫（子オブジェクトの子オブジェクト)を取得する。
        //以下の場合なら自身の子オブジェクトChildの子オブジェクトGrandChildを取得
        public Transform headChild;
    public Transform GunPositon;
    public Transform[] Shoulder;

    // Direction constants
    private const int None = -1;
    private const int Idle = 0;
    private const int Forward = 1;
    private const int ForwardRight = 2;
    private const int Right = 3;
    private const int BackwardRight = 4;
    private const int Backward = 5;
    private const int BackwardLeft = 6;
    private const int Left = 7;
    private const int ForwardLeft = 8;

    private const int ChangeMotionNum = 8;

    private const int WalkMotionNum = 1;
    private const int CrouchMotionNum = 2;

    private Rigidbody rb;

    private bool isJumping = false;
    private bool isGrounded = true;


    private Vector3 lastMoveDirection; // Store the last move direction

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            health = HEALTH;
            m_StateSkill = StateSkill.COUNT;
            for (int i = 0; i < m_Skill_Info.Length; i++)
            {
                m_Skill_Info[i].skill.ResetSkill();
            }

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true; // Prevent rotation from physics

            PhotonNetwork.LocalPlayer.TagObject = this;
            //ネットワークで銃を作成する
            gunInstance = PhotonNetwork.Instantiate(GunPrefab.name, GunPositon.position, Shoulder[0].rotation);
            //プレイヤーを子にする
            photonView.RPC("SetParentRPC", RpcTarget.AllBuffered, gunInstance.GetPhotonView().ViewID, photonView.ViewID);
            // Setting the initial grounded state
            isGrounded = true;

            // スライダーを取得する
            slider = GameObject.Find("PlayerHpBar").GetComponent<Slider>();

        }
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;
    }
    //
    [PunRPC]
    void SetParentRPC(int gunViewID, int parentViewID)
    {
        PhotonView gunView = PhotonView.Find(gunViewID);
        PhotonView parentView = PhotonView.Find(parentViewID);

        if (gunView != null && parentView != null)
        {
            GameObject gunObj = gunView.gameObject;
            GameObject parentObj = parentView.gameObject;

            gunObj.transform.SetParent(parentObj.transform);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();

            gunInstance.transform.position = GunPositon.position;
            gunInstance.transform.rotation = Shoulder[0].rotation;
            BaseGun baseGun = null;

            baseGun = gunInstance.GetComponent<BaseGun>();

            baseGun.StateUpdate();

            if(health <= 0)
            {
                transform.position = new Vector3(0, 0, 0);

                health = HEALTH;
            }

            slider.value = health / HEALTH;
        }
    }

    private void HandleInput()
    {
        var input = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
            input += new Vector3(-1, 0, 0);

        if (Input.GetKey(KeyCode.D))
            input += new Vector3(1, 0, 0);

        if (Input.GetKey(KeyCode.W))
            input += new Vector3(0, 0, 1);

        if (Input.GetKey(KeyCode.S))
            input += new Vector3(0, 0, -1);

        // 歩いていたら
        if (input != Vector3.zero)
        {
            input.Normalize();

            if (!isJumping)
            {
                // Update animation
                UpdateAnimation(input);
            }
        }

        animator.SetBool("CrouchFlag", false);
        if (Input.GetKey(KeyCode.LeftControl))
        {
            input *= CrouchSpeedRatio;
            animator.SetInteger("Direction", animator.GetInteger("Direction") + ChangeMotionNum * CrouchMotionNum);
            animator.SetBool("CrouchFlag", true);


        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            input *= WalkSpeedRatio;
            animator.SetInteger("Direction", animator.GetInteger("Direction") + ChangeMotionNum * WalkMotionNum);

        }

        // 歩いていたら
        if (input == Vector3.zero)
            if (!isJumping)
                animator.SetInteger("Direction", Idle);


        // Convert input to world space relative to the camera
        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;
        camForward.y = 0; // Keep movement on the ground plane
        camRight.y = 0;
        if (isGrounded)
        {
            Vector3 moveDirection = (camForward * input.z + camRight * input.x).normalized * RunSpeed;
            // Apply movement with ground friction
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
            lastMoveDirection = input; // Store last move direction
        }
        else
        {
            Vector3 moveDirection = (camForward * lastMoveDirection.z + camRight * lastMoveDirection.x).normalized * RunSpeed;
            // Apply movement with reduced control in the air
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        }


        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }
        
        // Skill handling
        for (int i = 0; i < m_Skill_Info.Length; i++)
        {
            m_Skill_Info[i].skill.MUpdate(this);
            if (Input.GetKeyDown(m_Skill_Info[i].skill_Key))
            {
                m_Skill_Info[i].skill.Activate(this);
                m_StateSkill = (StateSkill)i;
            }
        }
        if ((int)m_StateSkill < m_Skill_Info.Length)
            m_Skill_Info[(int)m_StateSkill].skill.StateUpdate(this);
    }

    private void UpdateAnimation(Vector3 input)
    {
        // Update animation based on movement direction
        if (input == Vector3.forward)
        {
            animator.SetInteger("Direction", Forward);
        }
        else if (input == (Vector3.forward + Vector3.right).normalized)
        {
            animator.SetInteger("Direction", ForwardRight);
        }
        else if (input == Vector3.right)
        {
            animator.SetInteger("Direction", Right);
        }
        else if (input == (Vector3.right + Vector3.back).normalized)
        {
            animator.SetInteger("Direction", BackwardRight);
        }
        else if (input == Vector3.back)
        {
            animator.SetInteger("Direction", Backward);
        }
        else if (input == (Vector3.back + Vector3.left).normalized)
        {
            animator.SetInteger("Direction", BackwardLeft);
        }
        else if (input == Vector3.left)
        {
            animator.SetInteger("Direction", Left);
        }
        else if (input == (Vector3.left + Vector3.forward).normalized)
        {
            animator.SetInteger("Direction", ForwardLeft);
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Jump");
        isJumping = true;
        isGrounded = false; // Set to false to disable movement input during the jump
        animator.SetInteger("Direction", None);
    }

    // This method should be called when the jump animation finishes
    public void OnLanding()
    {
        isJumping = false;
        isGrounded = true;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // ダメージを受ける
        health -= damage;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnLanding();
        }

    }

}
