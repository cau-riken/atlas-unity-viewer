using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowLeftClickController : MonoBehaviour
{
    bool leftOnly = false;
    private List<GameObject> objList_right;

    void Awake()
    {
        GameObject[] objList = FindObjectsOfType<GameObject>();
        objList_right = new List<GameObject>();

        for (var i = 0; i < objList.Length; i++)
        {
            if (objList[i].name.Contains("RH"))
            {
                objList_right.Add(objList[i]);
            }
        }
    }
    public void HideRight()
    {
        if (leftOnly == false)
        {
            GameObject.Find("Show_left_button").GetComponentInChildren<Text>().text = "Show Whole Brain";
            leftOnly = true;

            for (var i = 0; i < objList_right.Count; i++)
            {   
                MeshRenderer render = objList_right[i].GetComponentInChildren<MeshRenderer>();
                render.enabled = false;
                objList_right[i].GetComponent<MeshCollider>().enabled = false;               
            }
        }
        else
        {
            GameObject.Find("Show_left_button").GetComponentInChildren<Text>().text = "Show Left Hemisphere";
            leftOnly = false;

            for (var i = 0; i < objList_right.Count; i++)
            {               
                MeshRenderer render = objList_right[i].GetComponentInChildren<MeshRenderer>();
                render.enabled = true;
                objList_right[i].GetComponent<MeshCollider>().enabled = true;                  
            }
        }
    }
}