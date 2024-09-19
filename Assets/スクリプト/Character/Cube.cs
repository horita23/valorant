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
    Skill_1 = 0,
    Skill_2 = 1,
    Gun     = 3,
    knife   = 4,
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

    [Tooltip("�e�̎��")]
    public GameObject GunPrefab;
    private GameObject gunInstance;

    [Tooltip("The second skill of the character.")]
    public Skill_Info[] m_Skill_Info;

    // Jump parameters
    public float jumpForce = 10f;

    private StateSkill m_StateSkill;

    private Animator animator = null;

    private Slider slider;

        //���i�q�I�u�W�F�N�g�̎q�I�u�W�F�N�g)���擾����B
        //�ȉ��̏ꍇ�Ȃ玩�g�̎q�I�u�W�F�N�gChild�̎q�I�u�W�F�N�gGrandChild���擾
        public Transform headChild;
    public Transform GunPositon;
    public Transform[] Shoulder;
    public Transform[] ReconColliderPositon;

    public GameObject FlashImgPrefab;
    private Image flashImg;
    private bool[] flashHitFlag = new bool [2];

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

    public float MouseSensitivity = 10f;

    private float horizontalRotation;


    private Vector3 lastMoveDirection; // Store the last move direction

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            health = HEALTH;
            m_StateSkill = StateSkill.knife;
            for (int i = 0; i < m_Skill_Info.Length; i++)
            {
                m_Skill_Info[i].skill.initialize(this, m_Skill_Info[i].skill_Key);

            }

            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true; // Prevent rotation from physics

            PhotonNetwork.LocalPlayer.TagObject = this;

           isGrounded = true;

            // �X���C�_�[���擾����
            slider = GameObject.Find("PlayerHpBar").GetComponent<Slider>();

            // �܂�Scene����Canvas��T���܂�
            Canvas canvas = FindObjectOfType<Canvas>();

            // FlashImgPrefab��Canvas�̎q�v�f�Ƃ��Đ������܂�
            GameObject flashImgObject = Instantiate(FlashImgPrefab, canvas.transform);


            // �C���X�^���X����Image�R���|�[�l���g���擾
            flashImg = flashImgObject.GetComponent<Image>();

            flashImg.color = Color.clear;

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
    [PunRPC]
    private void UnsetParentRPC(int ViewID)
    {
        PhotonView flashModelView = PhotonView.Find(ViewID);

        if (flashModelView != null)
        {
            flashModelView.transform.SetParent(null);
        }
    }

    public void AddItem(AK item)
    {
        //�l�b�g���[�N�ŏe���쐬����
        gunInstance = PhotonNetwork.Instantiate(item.name, GunPositon.position, Shoulder[0].rotation);
        //�v���C���[���q�ɂ���
        photonView.RPC("SetParentRPC", RpcTarget.AllBuffered, gunInstance.GetPhotonView().ViewID, photonView.ViewID);
        m_StateSkill = StateSkill.Gun;
        gunInstance.SetActive(true);


    }
    // Update is called once per frame
    void Update()
    {
        if (gunInstance)
        {
            if (photonView.IsMine)
            {
                HandleInput();

                gunInstance.transform.position = GunPositon.position;
                if (health <= 0)
                {
                    transform.position = new Vector3(0, 0, 0);

                    health = HEALTH;
                }

                slider.value = health / HEALTH;

                // �v���C���[�̃J�X�^���v���p�e�B���擾
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isFlashed", out object flashed))
                {
                    // �擾�����I�u�W�F�N�g�� bool[] �z��ł���Ɖ���
                    bool[] flashedArray = flashed as bool[];
                    if (flashedArray != null)
                    {
                        // �z��̑S�v�f���擾���A��������
                        for (int i = 0; i < flashedArray.Length; i++)
                        {
                            flashHitFlag[i] = flashedArray[i];

                            // �����Ŋe�v�f�Ɋ�Â���������ǉ��ł��܂�
                        }
                    }
                }

                // UI��\��
                if (flashHitFlag[0])
                {
                    flashImg.color = new Color(0, 1, 0, 1); // �t���b�V���̐F
                }
                else
                {
                    flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime * 0.5f); // �F���N���A
                }

                if (flashHitFlag[1])
                {
                    flashImg.color = new Color(1, 1, 1, 1); // �t���b�V���̐F
                }
                else
                {
                    flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime); // �F���N���A
                }

            }
        }
        else
        {

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

        //�L������]����
        {
            // �}�E�X���͂ɂ��J�����̉�]
            float mouseX = Input.GetAxis("Mouse X");
            horizontalRotation += mouseX * MouseSensitivity;

            // �v���C���[��Avatar�I�u�W�F�N�g���J�����̐�����]�ɍ��킹�ĉ�]
            transform.rotation = Quaternion.Euler(0, horizontalRotation, 0); ;

        }

        // �����Ă�����
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

        // �����Ă�����
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

        //�e�̍X�V
        gunInstance.GetComponent<BaseGun>().MainUpdate();
        // Skill handling
        for (int i = 0; i < m_Skill_Info.Length; i++)
        {
            m_Skill_Info[i].skill.MUpdate(this);

            if (Input.GetKeyDown(m_Skill_Info[i].skill_Key))
            {
                m_StateSkill = (StateSkill)i;
                m_Skill_Info[i].skill.Activate(this);
                gunInstance.SetActive(false);
                animator.SetBool("GunHaveFlag", false);

            }
        }
        //�e�̑I��
        if (Input.GetKeyDown(KeyCode.T))
        {
            //�Ƃ肠�����X�L���̎��ɏe�̏��
            m_StateSkill = StateSkill.Gun;
            //�e�\��
            gunInstance.SetActive(true);
            animator.SetBool("GunHaveFlag", true);

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_StateSkill = StateSkill.knife;
            gunInstance.SetActive(false);
            animator.SetBool("GunHaveFlag", false);

        }

        //�ǂ̎莝���̏�Ԃ�
        switch (m_StateSkill)
        {
            case StateSkill.Gun:
                gunInstance.GetComponent<BaseGun>().StateUpdate();
                break;

            case StateSkill.knife:
                break;

            case StateSkill.Skill_1:
                m_Skill_Info[(int)StateSkill.Skill_1].skill.StateUpdate(this);

                break;

            case StateSkill.Skill_2:
                m_Skill_Info[(int)StateSkill.Skill_2].skill.StateUpdate(this);

                break;
        }

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
        // �_���[�W���󂯂�
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
