using System;
using UnityEngine;

public class GeneralPool : MonoBehaviour
{
    //[Header("--- Pooled Object ---")]


    [Header("Bullet")]
    public GameObject bullet;
    public int bulletObjCountInPool;

    public static ObjectPooling bulletPool;


    void Start()
    {
        FillPool();
    }


    private void FillPool()
    {
        bulletPool = new ObjectPooling(bullet, transform);
        bulletPool.HavuzuDoldur(bulletObjCountInPool);
    }
    /*public static void FlashEffect(Vector3 pos, float dieTime)
    {
        GameObject a = bulletPool.HavuzdanObjeCek();

        a.transform.position = pos;

        a.GetComponent<Bullet>().dieTime = dieTime;
        a.GetComponent<Bullet>().pool = bulletPool;
    }*/
}




public class ObjectsProperties
{
    [SerializeField] private GameObject pooledObject;
    [SerializeField] private int objectsCountInPool;

    public static ObjectPooling PoolName;
}