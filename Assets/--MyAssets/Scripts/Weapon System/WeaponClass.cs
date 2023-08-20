using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    public void Fire(Transform muzzle, GameObject bullet, float bulletSpeed, float destroyTime)
    {
        GameObject insBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);

        insBullet.GetComponent<Bullet>().GoForward(bulletSpeed, muzzle);
        Destroy(insBullet, destroyTime);
    }

    public virtual void SayYourName(string name)
    {
        Debug.Log(name);
    }
}
