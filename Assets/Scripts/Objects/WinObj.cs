/*
Author: Carson L
Date: 6/1/2022
Desc: Script for object that makes the player win
*/

using System.Collections;
using System.Collections.Generic; using UnityEngine.SceneManagement;
using UnityEngine;

public class WinObj : MonoBehaviour
{
    private static SphereCollider sc;
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        sc = gameObject.GetComponent<SphereCollider>();
        canvas.enabled = false;
    }

    private void OnTriggerEnter()
    {
        Cursor.lockState = CursorLockMode.None;
        canvas.enabled = true;

        Time.timeScale = 0f;
    }

    public void NextScene(string newScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(newScene);
    }

    public void MainMenu(int mainMenu = 0)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenu);
    }
}
