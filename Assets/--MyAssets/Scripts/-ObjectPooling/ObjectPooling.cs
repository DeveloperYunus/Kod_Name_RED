using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling
{
    private GameObject prefab;
    private Transform parent;
    private Stack<GameObject> objeHavuzu = new Stack<GameObject>();

    public ObjectPooling(GameObject prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;   
    }

    public void HavuzuDoldur(int miktar)
    {
        for (int i = 0; i < miktar; i++)
        {
            GameObject obje = Object.Instantiate(prefab, parent);
            HavuzaObjeEkle(obje);
        }
    }

    public GameObject HavuzdanObjeCek()
    {
        if (objeHavuzu.Count > 0)
        {
            GameObject obje = objeHavuzu.Pop();
            obje.SetActive(true);

            return obje;
        }

        return Object.Instantiate(prefab, parent);
    }

    public void HavuzaObjeEkle(GameObject obje)
    {
        obje.SetActive(false);
        objeHavuzu.Push(obje);
    }
}