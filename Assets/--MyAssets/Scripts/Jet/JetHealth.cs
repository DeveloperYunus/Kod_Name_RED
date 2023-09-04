using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JetHealth : MonoBehaviour
{
    [Header("Health System")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float armour;
    [SerializeField] private float dieCrashImpulse;

    float health;
    bool live = true;

    [Header("Crash")]
    [SerializeField] private GameObject crashExp;
    [SerializeField] private Transform jetCM, crashCM;                  //uçak zemine çarpýnca aktif olacak kamera
    [SerializeField] private float restartSceneTime;


    void Start()
    {
        health = maxHealth;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > dieCrashImpulse && live && !collision.collider.isTrigger)
        {
            live = false;

            GameObject exp = Instantiate(crashExp, transform.position, Quaternion.identity);

            jetCM.transform.SetParent(null);
            crashCM.transform.SetParent(null);
            crashCM.gameObject.SetActive(true);

            Invoke(nameof(RestartScene), restartSceneTime);
            gameObject.SetActive(false);
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
