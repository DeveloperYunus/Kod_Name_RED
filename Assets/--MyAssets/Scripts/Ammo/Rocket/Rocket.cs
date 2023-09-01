using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Rocket : MonoBehaviour, IKillable
{
    [Header("--- Rocket Stats ---")]
    public float Damage;
    public float RocketSpeed;
    public float RotateSensitivity;

    [SerializeField]private bool canRocketFollow;
    [SerializeField] private Transform target;

    [Header("--- Explosion VFX ---")]
    [SerializeField] private GameObject explosionEffect;

    public void Kill()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (canRocketFollow)
        {
            if (target == null)
            {
                canRocketFollow = false;
                return;
            }
            FollowTheTarget();
        }
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

    private void FollowTheTarget()
    {
        transform.DOLookAt(target.position, RotateSensitivity);
        GetComponent<Rigidbody>().velocity = transform.forward * RocketSpeed;
    }

    public IEnumerator RocketVelocity(float timer, bool canRocketFollowTarget, Transform lockedObject)
    {
        yield return new WaitForSeconds(timer);

        canRocketFollow = canRocketFollowTarget;
        target = lockedObject;

        if (!canRocketFollow)
        {
            GetComponent<Rigidbody>().velocity = transform.forward * RocketSpeed;
        }
    }
}
