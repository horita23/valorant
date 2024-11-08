using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.AI;

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
    public float COOL_TIME = 3f;
    private float coolTime;

    public enum Team
    {
        TeamA,
        TeamB
    }
    public  bool GameEndFlag = false;
    public Team team;
    public Vector3 respawnPositon;

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
    public GameObject gunInstance;

    [Tooltip("The second skill of the character.")]
    public Skill_Info[] m_Skill_Info;

    // Jump parameters
    public float jumpForce = 10f;

    private StateSkill m_StateSkill;

    private Animator animator = null;

    private Slider slider;

    public int killCount = 0;

        //孫（子オブジェクトの子オブジェクト)を取得する。
        //以下の場合なら自身の子オブジェクトChildの子オブジェクトGrandChildを取得
        public Transform headChild;
    public Transform GunPositon;
    public Transform[] Shoulder;
    public Transform[] ReconColliderPositon;
    public Transform CameraPosition;

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

    private Vector4 FlashColor;

    // Start is called before the first frame update
    void Start()
    {

        if (photonView.IsMine)
        {
            coolTime = COOL_TIME;
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

            // スライダーを取得する
            slider = GameObject.Find("PlayerHpBar").GetComponent<Slider>();

            // まずScene内のCanvasを探します
            Canvas canvas = FindObjectOfType<Canvas>();

            // FlashImgPrefabをCanvasの子要素として生成します
            GameObject flashImgObject = Instantiate(FlashImgPrefab, canvas.transform);


            // インスタンスからImageコンポーネントを取得
            flashImg = flashImgObject.GetComponent<Image>();

            flashImg.color = Color.clear;
            //キャラクターの子オブジェクトにアタッチされているCameraコンポーネントを無効化
            Camera characterCamera = GetComponentInChildren<Camera>();
            if (characterCamera != null)
            {
                characterCamera.enabled = true;
            }

            // カスタムプロパティにViewIDを保存
            ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            // プレイヤーのActorNumberをキーにしてViewIDを保存
            customProperties[$"killCount_{PhotonNetwork.LocalPlayer.ActorNumber}"] = killCount;
            customProperties[$"Teme_{PhotonNetwork.LocalPlayer.ActorNumber}"] = team;
            customProperties[$"GameEneFlag_{PhotonNetwork.LocalPlayer.ActorNumber}"] = GameEndFlag;

            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);


        }
        PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 30;
        
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
        //ネットワークで銃を作成する
        gunInstance = PhotonNetwork.Instantiate(item.name, GunPositon.position, Shoulder[0].rotation);
        //プレイヤーを子にする
        photonView.RPC("SetParentRPC", RpcTarget.AllBuffered, gunInstance.GetPhotonView().ViewID, photonView.ViewID);
        m_StateSkill = StateSkill.Gun;
        animator.SetBool("GunHaveFlag", true);



    }
    // Update is called once per frame
    void Update()
    {

        ExitGames.Client.Photon.Hashtable gameEndFlag = PhotonNetwork.LocalPlayer.CustomProperties;
        // プレイヤーのActorNumberをキーにしてViewIDを保存
        GameEndFlag = (bool)gameEndFlag[$"GameEneFlag_{PhotonNetwork.LocalPlayer.ActorNumber}"];

        if (GameEndFlag)
        {
            return;

        }

        if (gunInstance)
        {
            if (photonView.IsMine)
            {
                //
                if (coolTime >= 0)
                {
                    coolTime -= Time.deltaTime;
                    health = HEALTH;
                }

                    // フラッシュUIを表示
                    if (flashHitFlag[0])
                {
                    flashImg.color = new Color(0, 1, 0, 1); // フラッシュの色
                }
                else
                {
                    flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime * 0.5f); // 色をクリア
                }

                if (flashHitFlag[1])
                {
                    flashImg.color = new Color(1, 1, 1, 1); // フラッシュの色
                }
                else
                {
                    flashImg.color = Color.Lerp(flashImg.color, Color.clear, Time.deltaTime); // 色をクリア
                }

                //プレイヤー色々な操作
                HandleInput();
                
                gunInstance.transform.position = GunPositon.position;
                //HPが０ならリスポーン位置に移動
                if (health <= 0)
                {
                    
                    transform.position = respawnPositon;

                    health = HEALTH;

                    killCount++;
                    coolTime = COOL_TIME;

                    // カスタムプロパティにViewIDを保存
                    ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
                    // プレイヤーのActorNumberをキーにしてViewIDを保存
                    customProperties[$"killCount_{PhotonNetwork.LocalPlayer.ActorNumber}"] = killCount;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

                    if (killCount != (int)customProperties[$"killCount_{PhotonNetwork.LocalPlayer.ActorNumber}"])
                    {
                        // プレイヤーのActorNumberをキーにしてViewIDを保存
                        customProperties[$"killCount_{PhotonNetwork.LocalPlayer.ActorNumber}"] = killCount;
                        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);

                    }
                }
                //HPバー
                slider.value = health / HEALTH;



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

        //キャラ回転処理
        {
            // マウス入力によるカメラの回転
            float mouseX = Input.GetAxis("Mouse X");
            horizontalRotation += mouseX * MouseSensitivity;

            // プレイヤーのAvatarオブジェクトをカメラの水平回転に合わせて回転
            transform.rotation = Quaternion.Euler(0, horizontalRotation, 0); ;

        }

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

        //銃の更新
        gunInstance.GetComponent<BaseGun>().MainUpdate();
        // Skill handling
        for (int i = 0; i < m_Skill_Info.Length; i++)
        {
            m_Skill_Info[i].skill.MUpdate(this);
            if (!m_Skill_Info[0].skill.SkillActivation && !m_Skill_Info[1].skill.SkillActivation)
            {
                if (Input.GetKeyDown(m_Skill_Info[i].skill_Key))
                {
                    m_StateSkill = (StateSkill)i;
                    m_Skill_Info[i].skill.Activate(this);
                    gunInstance.SetActive(false);
                    animator.SetBool("GunHaveFlag", false);

                }
            }
        }
        //銃の選択
        if (Input.GetKeyDown(KeyCode.T))
        {
            //とりあえずスキルの次に銃の状態
            m_StateSkill = StateSkill.Gun;
            m_Skill_Info[0].skill.resetSkill(this);
            if(!m_Skill_Info[1].skill.SkillActivation) m_Skill_Info[1].skill.resetSkill(this);


            //銃表示
            gunInstance.SetActive(true);
            animator.SetBool("GunHaveFlag", true);

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_StateSkill = StateSkill.knife;
            m_Skill_Info[0].skill.resetSkill(this);
            if (!m_Skill_Info[1].skill.SkillActivation) m_Skill_Info[1].skill.resetSkill(this);

            gunInstance.SetActive(false);
            animator.SetBool("GunHaveFlag", false);

        }

        //どの手持ちの状態か
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
        if(coolTime <= 0)
            // ダメージを受ける
            health -= damage;

    }

    [PunRPC]
    public void Flash(bool HitFlash = false,bool NoHitFlash = false)
    {
        flashHitFlag[0] = HitFlash;
        flashHitFlag[1] = NoHitFlash;



    }

    public int GetKillCount()
    {
        return killCount;
    }

    public Team GetTeam() { return team; }

    public float GetHp() {        return health; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnLanding();
        }

    }

}
