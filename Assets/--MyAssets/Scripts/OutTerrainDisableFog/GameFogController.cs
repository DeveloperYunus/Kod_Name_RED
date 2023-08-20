using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameFogController : MonoBehaviour
{
    [SerializeField] private Volume seaFog;
    [SerializeField] private Volume desertFog;
    [SerializeField] private float fogChangeAmount;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<JetController>())
        {
            CloseSeaFog();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<JetController>())
        {
            OpenSeaFog();   
        }
    }

    void OpenSeaFog()
    {
        while (seaFog.weight < desertFog.weight)
        {
            seaFog.weight += fogChangeAmount * Time.deltaTime;
        }
    }
    void CloseSeaFog()
    {
        while (seaFog.weight > 0)
        {
            seaFog.weight -= fogChangeAmount * Time.deltaTime;
        }
    }
}
