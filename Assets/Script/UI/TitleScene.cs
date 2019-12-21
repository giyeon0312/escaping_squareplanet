using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public string sceneName = "MainScene";
    [SerializeField]
    private GameObject storyPanel;

    public void ClickStart()
    {
        Debug.Log("로딩");
        SceneManager.LoadScene(sceneName);
    }

    public void ClickStory()
    {
        Debug.Log("스토리 보여주기");
        storyPanel.SetActive(true);
    }

    public void ClickCloseStory()
    {
        storyPanel.SetActive(false);
    }
    

    public void ClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
