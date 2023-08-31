using UnityEngine;

public class Rocket : MonoBehaviour, IKillable
{
    [Header("--- Rocket Stats ---")]
    public float Damage;

    [Header("--- Explosion VFX ---")]
    [SerializeField] private GameObject explosionEffect;

    public void Kill()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageTakeable<float>>();
        if (damageable != null)
        {
            GetComponent<Collider>().enabled = false;
            Kill();
            damageable.TakeDamage(Damage);
        }


        if (!other.GetComponent<Collider>().isTrigger && other.GetComponent<IIgnorableForRocket>() == null) 
        {
            Kill();
        }
    }
}
