/*
Author: Carson L
Date: 5/16/2023
Desc: scene data handler
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;

public class StoreData : MonoBehaviour
{
    [SerializeField]
     public GameObject[] SceneDataChange;
    [SerializeField]
     public float[] SceneDataDelete;

    private GameObject[] SceneData;
    // Start is called before the first frame update
    void Start()
    {
        SceneDataChange = GameObject.FindGameObjectsWithTag("AddData");
    }

    // Update is called once per frame
    void Update()
    {
        SceneData = GameObject.FindGameObjectsWithTag("GameManager");

        if(!(SceneData.Length > 1))
        {
            DontDestroyOnLoad(gameObject);
        }

    }
    public void FindObjects()
    {
        SceneDataChange = GameObject.FindGameObjectsWithTag("AddData");
        for (int i = 0; i < SceneDataChange.Length; i++)
        {
            SceneDataChange[i].GetComponent<Slider>().value = SceneDataDelete[i];
        }
    }

    public void GetData()
    {
        for (int i = 0; i < SceneDataChange.Length; i++)
        {
            SceneDataDelete[i] = SceneDataChange[i].GetComponent<Slider>().value;
        }
    }
}


