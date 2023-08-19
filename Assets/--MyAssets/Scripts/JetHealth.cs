using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class JetHealth : MonoBehaviour
{
    [Header("Health System")]
    public float maxHealth;
    public float armour;
    public float dieCrashImpulse;

    float health;
    bool live = true;

    [Header("Crash")]
    public GameObject crashExp;
    public Transform jetCM, crashCM;                  //uçak zemine çarpýnca aktif olacak kamera


    void Start()
    {
        health = maxHealth;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > dieCrashImpulse && live)
        {
            live = false;

            GameObject exp = Instantiate(crashExp, transform.position, Quaternion.identity);

            jetCM.transform.SetParent(null);
            crashCM.transform.SetParent(null);
            crashCM.gameObject.SetActive(true);

            Destroy(gameObject);
        }
    }
}
