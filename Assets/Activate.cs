using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{
    public GameObject SceneData;
    // Start is called before the first frame update
    void Start()
    {
        SceneData = GameObject.FindGameObjectWithTag("GameManager");
        SceneData.GetComponent<StoreData>().FindObjects();
    }


    public void OnEnable()
    {
        SceneData = GameObject.FindGameObjectWithTag("GameManager");
        SceneData.GetComponent<StoreData>().FindObjects();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
