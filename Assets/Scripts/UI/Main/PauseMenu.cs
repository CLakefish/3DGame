/*
 * zak baydass
 * 10/28/2022
 * this script is what the Menu system uses to enable the pause menu and settings menu
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GameisPaused = false;
    public static bool SettingsisOpen = false;
    public static bool canPause = false;
    public bool isPaused = false;


    public GameObject ClosingUI;
    public GameObject OpeningUI;
    public GameObject PauseUI;


    public GameObject pauseFirstButton;
    private void Awake()
    {
        canPause = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            onPause();
        }

    }
    public void onPause() //uses new input system to pause game by setting value to true or false. 
    {
        if (canPause == true && SettingsisOpen == false)
        {
            if (GameisPaused == true && canPause == true)
            {
                Resume();
            }
            else if (GameisPaused == false && canPause == true)
            {
                Pause();
            }
        }
        Debug.Log("pressed Pause");
    }
    public void Resume()
    {
        PauseUI.SetActive(false);
        GameisPaused = false;
        isPaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;

    }
    void Pause()
    {
        PauseUI.SetActive(true);
        GameisPaused = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        isPaused = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }
    public void OpenSettings()
    {
        SettingsisOpen = true;
        OpeningUI.SetActive(true);
        ClosingUI.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }
    public void CloseSettings()
    {
        SettingsisOpen = false;
        OpeningUI.SetActive(false);
        ClosingUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }
    public void StartGame()
    {

    }
}
