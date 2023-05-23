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

    public PlayerCamera PlayerCamera;

    public GameObject player;
    public Vector3 Vel;

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
        player = GameObject.FindGameObjectWithTag("Player");
        if (canPause == true && SettingsisOpen == false)
        {
            if (GameisPaused == true && canPause == true)
            {
                Resume();
                if(player != null)
                player.GetComponent<Rigidbody>().velocity = Vel;
            }
            else if (GameisPaused == false && canPause == true)
            {
                Pause();
                if(player != null)
                Vel = player.GetComponent<Rigidbody>().velocity;
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
        //PlayerCamera.CanRotate = true;
    }
    void Pause()
    {
        PauseUI.SetActive(true);
        GameisPaused = true;
        isPaused = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        //PlayerCamera.CanRotate = false;
    }
    public void OpenSettings()
    {
        SettingsisOpen = true;
        OpeningUI.SetActive(true);
        ClosingUI.SetActive(false);
    }
    public void CloseSettings()
    {
        SettingsisOpen = false;
        OpeningUI.SetActive(false);
        ClosingUI.SetActive(true);
    }
    public void StartGame()
    {

    }
}
