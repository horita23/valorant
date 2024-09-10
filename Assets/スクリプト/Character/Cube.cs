using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    public float health = 100.0f;

    [Tooltip("The movement speed of the character.")]
    public float speed = 5f;

    [Tooltip("�e�̎��")]
    public GameObject GunPrefab;
    private GameObject gunInstance;

    [Tooltip("The second skill of the character.")]
    public Skill_Info[] m_Skill_Info;

    private int m_StateSkill;

    private Animator animator = null;


    //���i�q�I�u�W�F�N�g�̎q�I�u�W�F�N�g)���擾����B
    //�ȉ��̏ꍇ�Ȃ玩�g�̎q�I�u�W�F�N�gChild�̎q�I�u�W�F�N�gGrandChild���擾
    public Transform headChild;

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

    private Rigidbody rb;

    // Jump parameters
    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    public float gravityMultiplier = 1f;
    private bool isJumping = false;
    private bool isGrounded = true;

    public float airControl = 0.3f;    // Control multiplier while in the air

    private Vector3 lastMoveDirection; // Store the last move direction

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            m_StateSkill = -1;
            for (int i = 0; i < m_Skill_Info.Length; i++)
            {
                m_Skill_Info[i].skill.ResetSkill();
            }
            //�l�b�g���[�N�ŏe���쐬����
            gunInstance = PhotonNetwork.Instantiate(GunPrefab.name, transform.position, transform.rotation);
            gunInstance.SetActive(false);
            //�v���C���[���q�ɂ���
            photonView.RPC("SetParentRPC", RpcTarget.AllBuffered, gunInstance.GetPhotonView().ViewID, photonView.ViewID);

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true; // Prevent rotation from physics

            PhotonNetwork.LocalPlayer.TagObject = this;

            // Setting the initial grounded state
            isGrounded = true;
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

        // Normalize the input to ensure consistent movement speed
        if (input != Vector3.zero)
        {
            input.Normalize();
        }

        // Handle jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            Jump();
        }



        //�W�����v
        Move(input);

        //�W�����v����Ȃ�������A�j���[�V����
        if (!isJumping)
        {
            // Update animation
            UpdateAnimation(input);
        }


        // Skill handling
        for (int i = 0; i < m_Skill_Info.Length; i++)
        {
            m_Skill_Info[i].skill.MUpdate(this);

            if (Input.GetKeyDown(m_Skill_Info[i].skill_Key))
            {
                m_Skill_Info[i].skill.Activate(this);
                m_StateSkill = i;
                gunInstance.SetActive(false);
            }
        }

        //�e�̍X�V
        gunInstance.GetComponent<BaseGun>().MainUpdate();
        //�e�̑I��
        if (Input.GetKeyDown(KeyCode.T))
        {
            //�Ƃ肠�����X�L���̎��ɏe�̏��
            m_StateSkill = m_Skill_Info.Length;
            //�e�\��
            gunInstance.SetActive(true);
        }

        //�ǂ̎莝���̏�Ԃ�
        if (0 <= m_StateSkill && m_StateSkill< m_Skill_Info.Length)
            m_Skill_Info[m_StateSkill].skill.StateUpdate(this);

        else if(m_StateSkill == m_Skill_Info.Length)
            //�e�̍X�V
            gunInstance.GetComponent<BaseGun>().StateUpdate();
    }

    //�ړ��A�j���[�V����
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
        else
        {
            animator.SetInteger("Direction", Idle);
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

    //�W�����v�̍X�V
    void Move(Vector3 input)
    {
        // Convert input to world space relative to the camera
        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;

        camForward.y = 0; // Keep movement on the ground plane
        camRight.y = 0;
        if (isGrounded)
        {
            Vector3 moveDirection = (camForward * input.z + camRight * input.x).normalized * speed;
            // Apply movement with ground friction
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
            lastMoveDirection = input; // Store last move direction
        }
        else
        {
            input *= airControl;
            Vector3 moveDirection = (camForward * (lastMoveDirection.z + input.z) + camRight * (lastMoveDirection.x + input.x)) * speed;
            // Apply movement with reduced control in the air
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        }

    }

    // This method should be called when the jump animation finishes
    public void OnLanding()
    {
        isJumping = false;
        isGrounded = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnLanding();
        }
    }
}
