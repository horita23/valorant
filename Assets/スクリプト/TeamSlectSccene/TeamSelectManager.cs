using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeamSelectManager : MonoBehaviour
{

    public Button Abutton;
    public Button Bbutton;

    // Start is called before the first frame update
    void Start()
    {
        // ボタンにクリックイベントを登録
        Abutton.onClick.AddListener(SelectTeamA);
        Bbutton.onClick.AddListener(SelectTeamB);

    }

    // チームAを選択
    public void SelectTeamA()
    {
        PlayerPrefs.SetString("SelectedTeam", "TeamA");
        LoadPlayScene();

    }
    // チームBを選択
    public void SelectTeamB()
    {
        PlayerPrefs.SetString("SelectedTeam", "TeamB");
        LoadPlayScene();
    }

    // プレイシーンに遷移
    private void LoadPlayScene()
    {
        SceneManager.LoadScene("LowPolyFPS_Lite_Demo");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
