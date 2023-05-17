using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DevMode : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] KeyCode open;
    [SerializeField] KeyCode spawnObj;

    [Header("UI")]
    [SerializeField] TMP_Text statusText;
   // [SerializeField] GameObject infoPanel;
    bool isActive = false;

    Camera cam;
    PlayerControls player;
    PlayerHealth playerHealth;

    private void Start()
    {
        cam = FindObjectOfType<Camera>().GetComponent<Camera>();

        player = FindObjectOfType<PlayerControls>().GetComponent<PlayerControls>();
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(open))
        {
            isActive = isActive ? false : true;
        }

        statusText.text = isActive ? "enabled :)" : "disabled :(";
        statusText.color = isActive ? Color.green : Color.red;

        if (isActive)
        {
            if (Input.GetKeyDown(spawnObj))
            {
                GameObject obj = FindObjectOfType<Enemy>().gameObject;

                GameObject newObj = Instantiate(obj, transform.position, Quaternion.identity);
                newObj.transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonDown(1)) playerHealth.Hit(Random.Range(0, 25), transform.position, 1f);
            if (Input.GetMouseButtonDown(2)) playerHealth.GainHP(3);
        }
        else;
    }
}
