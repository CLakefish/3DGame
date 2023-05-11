using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreData : MonoBehaviour
{
    public GameObject[] SceneDataChange;

    public struct LoadedData
    {
        public GameObject objs;
        public int num;
        public int Number;
        public float FloatingNumber;
        public bool TOF;
    };
    public List<LoadedData> GameList;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneDataChange = GameObject.FindGameObjectsWithTag("AddData");
        //get all data 
        for (int i = 0; i < SceneDataChange.Length; i++)
        {
            for (int j = 0;j < GameList.Count;j++)
            {
                if (SceneDataChange[i].gameObject.GetComponent<Slider>() == GameList[j].objs.GetComponent<Slider>())
                {
                    SceneDataChange[i].GetComponent<Slider>().value = GameList[j].FloatingNumber;
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //upon scene change, check if same data can be sent out, send out if so


        //update data
    }

    public void updateData(GameObject obj, int NUm)
    {

        //get all data 
        for (int i = 0; i < SceneDataChange.Length; i++)
        {
            LoadedData LoadData = new LoadedData();
            LoadData.objs = obj;
            if (obj.GetComponent<Slider>())
            {
                LoadData.FloatingNumber = obj.GetComponent<Slider>().value;
                LoadData.num = NUm;
            }

            GameList.Add(LoadData);
        }

    }

}


