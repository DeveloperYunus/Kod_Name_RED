using UnityEngine;

public class WeaponClass : MonoBehaviour
{
    public void Fire(Transform muzzle, float bulletSpeed, float destroyTime, float bulletDamage)
    {
        GameObject insBullet = GeneralPool.bulletPool.HavuzdanObjeCek();

        insBullet.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
        insBullet.GetComponent<Bullet>().GoForward(bulletSpeed, muzzle);
        insBullet.GetComponent<Bullet>().SetDefaultProces(destroyTime, bulletDamage);
    }

    public virtual void SayYourName(string name)
    {
        Debug.Log(name);
    }
}
