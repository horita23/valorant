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
        // �{�^���ɃN���b�N�C�x���g��o�^
        Abutton.onClick.AddListener(SelectTeamA);
        Bbutton.onClick.AddListener(SelectTeamB);

    }

    // �`�[��A��I��
    public void SelectTeamA()
    {
        PlayerPrefs.SetString("SelectedTeam", "TeamA");
        LoadPlayScene();

    }
    // �`�[��B��I��
    public void SelectTeamB()
    {
        PlayerPrefs.SetString("SelectedTeam", "TeamB");
        LoadPlayScene();
    }

    // �v���C�V�[���ɑJ��
    private void LoadPlayScene()
    {
        SceneManager.LoadScene("LowPolyFPS_Lite_Demo");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
