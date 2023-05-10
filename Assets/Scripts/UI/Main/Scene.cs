/*
 * Name: Zak Baydass
 * Date: 10/4/22
 * Desc: add this to use the funtion in events in other components such as the menu buttons
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Threading;
using TMPro;

public class Scene : MonoBehaviour
{
    public string sceneName = "LevelOne";
    public string GameName = "GameEnded";
    public string GameTwo = "GameEndedPlayerTwo";


    public Slider VolumeSlider;
    public AudioMixer AudioMixer;

    public TMP_Text WinningText;

    public void setVolume() // sets volume in settings menu
    {
        AudioMixer.SetFloat("Volume",VolumeSlider.value);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public void ChangeScene() //loads a new scene
    {
        SceneManager.LoadScene(sceneName);
        PauseMenu.GameisPaused = false;
        PauseMenu.canPause = true;
        PauseMenu.SettingsisOpen = false;
        Time.timeScale = 1.0f;
    }
    public void LoseScene(int player) //loads the end of a game scene
    {
        if(player == 1)
            SceneManager.LoadScene(GameName);
        else if(player == 2)
            SceneManager.LoadScene(GameTwo);

    }
    public void EndGame()
    {
        Application.Quit();
    }


}
