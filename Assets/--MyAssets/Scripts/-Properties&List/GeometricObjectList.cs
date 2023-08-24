using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometricObjectList : MonoBehaviour
{
    public static GeometricObjectList Instance;


    [SerializeField] private int maxObjectAmount;
    [SerializeField] private Transform producedObjectHolder;
    private int _producedObj;

    public List<SampleObjetcs> SampleObjetcs = new();
    [HideInInspector] public string SelectedObjectName;


    private void Awake()
    {
        Instance = this;
        _producedObj = 0;

        SelectedObjectName = null;
    }
    private void OnMouseDown()
    {
        if (SelectedObjectName == null)
        {
            Debug.Log("You should select one object before you instantiate another one.");
            return;
        }

        for (int i = 0; i < SampleObjetcs.Count; i++)
        {
            Vector3 pos = producedObjectHolder.position + new Vector3(0, UnityEngine.Random.Range(1, 2), 0);

            if (SampleObjetcs[i].Name == SelectedObjectName)
            {
                Instantiate(SampleObjetcs[i].obj, pos, Quaternion.identity, producedObjectHolder);
                _producedObj++;

                ControlProducedObjectAmount();
            }
        }
    }

    private void ControlProducedObjectAmount()
    {
        if (_producedObj > maxObjectAmount)
        {
            Debug.Log("You reached Max object count. All object destroyd");
            _producedObj = 0;

            for (int i = 0; i < producedObjectHolder.childCount; i++)
            {
                Destroy(producedObjectHolder.GetChild(i).gameObject);
            }
        }
    }
}

[Serializable]
public class SampleObjetcs
{
    public GameObject obj;
    public string Name {
        get { return name; }
        set { name = value; }
    }
    [SerializeField] private string name;

    public int Price {
        get { return price; }
        set { price = value; } 
    }
    [SerializeField] private int price;
}