using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIManager : MonoBehaviour
{
    public Text playerHP;
    public Text RestBullet;
    public Text MaxBullet;
    public Text BulletBar;

    public Text[] Skill;
    public Text[] SkillCooldown;
    public Text[] SkillActive;

    // プレイヤー情報の更新メソッド
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerUI(float playerhP, int restbullet, int maxbulletUI,bool haveweapon)
    {
       // playerHP.text = playerhP.ToString();

        if (haveweapon)
        {
            RestBullet.text = restbullet.ToString();
            MaxBullet.text = maxbulletUI.ToString();
            RestBullet.enabled = true;
            MaxBullet.enabled = true;
            BulletBar.enabled = true;
        }
        else
        {
            RestBullet.enabled = false;
            MaxBullet.enabled = false;
            BulletBar.enabled = false;
        }
    }    // Start is called before the first frame update


}
