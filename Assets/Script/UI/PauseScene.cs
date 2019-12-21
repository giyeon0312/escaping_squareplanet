using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScene : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPause;

    void Start()
    {
        isPause = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPause)
                CallMenu();
            else
                CloseMenu();
        }
    }

    private void CallMenu()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; //메뉴가 호출된동안 시간이 멈추도록한다
        isPause = false;
    }

    private void CloseMenu()
    { 
        pausePanel.SetActive(false);
        Time.timeScale = 1f;//다시 정상속도로 흘러가도록
        isPause = true;
    }

    public void ClickExit()
    {
        Application.Quit();
    }
 }
