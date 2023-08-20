using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();    
    }
    public void GoForward(float speed, Transform muzzle)
    {
        rb.velocity = muzzle.forward * speed;
    }
}
