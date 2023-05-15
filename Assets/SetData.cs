using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetData : MonoBehaviour
{
    public GameObject GameData;
    public GameObject[] PlayerData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      GameData = GameObject.FindGameObjectWithTag("GameManager");
      PlayerData = GameObject.FindGameObjectsWithTag("AddData");

      GameData.GetComponent<StoreData>().SceneDataChange = PlayerData;

    }

    public void PressButton(bool Set)
    {
        if(Set)
        {
            GameData.GetComponent<StoreData>().SetData();
        }
        else
        {
            GameData.GetComponent<StoreData>().GetData();
        }


    }
}
