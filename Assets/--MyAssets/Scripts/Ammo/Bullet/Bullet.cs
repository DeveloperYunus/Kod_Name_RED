using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable, IDamageGivable<float>
{
    [HideInInspector] public ObjectPooling Pool;
    [HideInInspector] public float Damage;

    Rigidbody _rb;

    public void GiveDamage(float damageAmount)
    {

    }
    public void SentObjectToPool()
    {
        GetComponent<Bullet>().DieTimer();
    }



    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        _rb.velocity = Vector3.zero;
    }



    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Pool = GeneralPool.bulletPool;
    }


    public void GoForward(float speed, Transform muzzle)
    {
        _rb.velocity = muzzle.forward * speed;
    }


    public void SetDefaultProces(float dieTime, float damage)
    {
        Damage = damage;
        Invoke(nameof(DieTimer), dieTime);
    }
    public void DieTimer()
    {
        Pool.HavuzaObjeEkle(gameObject);
    }
}
