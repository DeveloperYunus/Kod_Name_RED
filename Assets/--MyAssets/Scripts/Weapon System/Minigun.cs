using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : WeaponClass
{
    public void ScreenShake()
    {
        Debug.Log("Screen shake");
    }
    public override void SayYourName(string name)
    {
        Debug.Log("Overrided minigun");
    }
}
