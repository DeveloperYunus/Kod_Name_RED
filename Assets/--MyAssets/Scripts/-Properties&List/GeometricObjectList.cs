using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GeometricObjectList : MonoBehaviour
{
    public static GeometricObjectList Instance;

    [SerializeField] private float range;
    [SerializeField] private Transform producedObjectHolder;
    private int producedObj;

    public List<SampleObjetcs> SampleObjetcs = new();
    [HideInInspector] public string SelectedObjectName;


    private void Awake()
    {
        Instance = this;
        producedObj = 0;

        SelectedObjectName = null;
    }


    public void ProduceObject()
    {
        if (SelectedObjectName == null)//This check is optimized (use it)
        {
            Debug.Log("You should select one object before you instantiate another one.");
            return;
        }

        for (int i = 0; i < SampleObjetcs.Count; i++)
        {
            Vector3 pos = producedObjectHolder.position + new Vector3(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(1, 3), UnityEngine.Random.Range(-range, range));

            if (SampleObjetcs[i].Name == SelectedObjectName)//change this control for id
            {
                Instantiate(SampleObjetcs[i].obj, pos, Quaternion.identity, producedObjectHolder);
                SayYourStats(i);
                producedObj++;
            }
        }
    }

    public void DestroyAllObject()
    {
        for (int i = 0; i < producedObjectHolder.childCount; i++)
        {
            producedObj = 0;

            Destroy(producedObjectHolder.GetChild(i).gameObject);
        }
    }

    private void SayYourStats(int numberInList)
    {
        string a = SampleObjetcs[numberInList].Name;
        int b = SampleObjetcs[numberInList].Price;
    }
}



[Serializable]
public class SampleObjetcs
{
    public GameObject obj;
    public string Name {
        get { 
            return name; 
        }
        set { name = value; }
    }
    [SerializeField] private string name;

    public int Price {
        get {
            Debug.Log("And my price is " + price + ".");
            return price; 
        }
        set { price = value; } 
    }
    [SerializeField] private int price;
}