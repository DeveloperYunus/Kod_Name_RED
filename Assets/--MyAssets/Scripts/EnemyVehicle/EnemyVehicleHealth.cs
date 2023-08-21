using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVehicleHealth : MonoBehaviour, IDamageTakeable<float>
{
    [SerializeField] private float maxHealth;

    //Soru : HP scriptinde collision-trigger hesabý yaparken collider ile hp script'inin ayný objede olmasý þartmý


    public void TakeDamage(float damage)
    {
        GetDamage(damage);
    }


    private void OnTriggerEnter(Collider other)
    {
        var giveDamage = other.GetComponent<Bullet>();
        if (giveDamage != null)
        {            
            other.GetComponent<Bullet>().SentObjectToPool();
            GetDamage(giveDamage.Damage);
        }
    }
    void GetDamage(float damage)
    {
        maxHealth -= damage;

        if (maxHealth < 0)
        {
            maxHealth = 0;
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
