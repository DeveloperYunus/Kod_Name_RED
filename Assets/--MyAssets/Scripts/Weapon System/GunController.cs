using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private GameObject bulletObj;

    [Header("----- GUNS -----")]

    [Header("--- Minigun ---")]
    [SerializeField] private Minigun minigun;
    [SerializeField] private Transform minigunMuzzle1;
    [SerializeField] private Transform minigunMuzzle2;
    [SerializeField] private float minigunBulletSpeed;
    [SerializeField] private float minigunBulletDestroyTime;
    [SerializeField] private float minigunAttackSpeed;
   
    [Header("--- Rifle ---")]
    [SerializeField] private Rifle rifle;
    [SerializeField] private Transform rifleMuzzle1;
    [SerializeField] private Transform rifleMuzzle2;
    [SerializeField] private float rifleBulletSpeed;
    [SerializeField] private float rifleBulletDestroyTime;
    [SerializeField] private float rifleAttackSpeed;

    private float attackSpeed;


    [Header("--- Texts ---")]
    [SerializeField] private TextMeshProUGUI activeGunText;

    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        minigun.enabled = true;
        rifle.enabled = false;
    }
    private void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0)
        {
            GunChanger();
        }

        if (Input.GetMouseButton(0))
        {   
            if (attackSpeed < 0)
            {
                FireBullet();
            }
            else
                attackSpeed -=Time.deltaTime;
            
        }
    }

    private void GunChanger()
    {
        minigun.enabled = !minigun.enabled;
        rifle.enabled = !rifle.enabled;

        activeGunText.text = "Active Gun : " + (minigun.enabled == true ? " Minigun" : "     Rifle");
    }

    private void FireBullet()
    {
        float jetSpeed = _rb.velocity.magnitude * 2;

        if (minigun.enabled)
        {
            minigun.Fire(minigunMuzzle1, bulletObj, minigunBulletSpeed + jetSpeed, minigunBulletDestroyTime);
            minigun.Fire(minigunMuzzle2, bulletObj, minigunBulletSpeed + jetSpeed, minigunBulletDestroyTime);

            minigun.SayYourName("unvalid");

            minigun.ScreenShake();

            attackSpeed = minigunAttackSpeed;
        }
        else
        {
            rifle.Fire(rifleMuzzle1, bulletObj, rifleBulletSpeed + jetSpeed, rifleBulletDestroyTime);
            rifle.Fire(rifleMuzzle2, bulletObj, rifleBulletSpeed + jetSpeed, rifleBulletDestroyTime);

            rifle.Sparkling();

            attackSpeed = rifleAttackSpeed;
        }
    }
}
