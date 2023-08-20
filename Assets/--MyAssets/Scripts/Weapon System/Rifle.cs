using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : WeaponClass
{
    public void Sparkling()
    {
        Debug.Log("Spark");
    }
    public override void SayYourName(string name)
    {
        Debug.Log("Overrided Rifle");

    }
}
