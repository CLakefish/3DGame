using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetData : MonoBehaviour
{
    public GameObject GameData;
    public GameObject[] PlayerData;

    // Update is called once per frame
    void Update()
    {
      GameData = GameObject.FindGameObjectWithTag("GameManager");
    }

    public void PressButton()
    {
            GameData.GetComponent<StoreData>().GetData();
    }
}
