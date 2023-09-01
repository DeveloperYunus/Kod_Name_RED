using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVehicleHealth : MonoBehaviour, IDamageTakeable<float>
{
    [SerializeField] private float maxHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthFilledImage;
    [SerializeField] private GameObject dieObject;


    [Header("--- Exp Effect ---")]
    [SerializeField] private GameObject expEffect;
    [SerializeField] private int expAmount;
    [SerializeField] private float expRangeX, expRangeY, expRangeZ;

    private float _health;

    //Soru : HP scriptinde collision-trigger hesabý yaparken collider ile hp script'inin ayný objede olmasý þartmý


    public static int executedEnemyCount;

    //------------------------------------------------------ Event

    public delegate void DestroyEnemyVehicle();
    public static event DestroyEnemyVehicle DestroyEnemy;

    public void TakeDamage(float damage)
    {
        GetDamage(damage);
    }

    private void Start()
    {
        _health = maxHealth;
        healthText.text = _health.ToString("0");

        executedEnemyCount = 0;
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
        _health -= damage;
        healthText.text = _health.ToString("0");
        healthFilledImage.fillAmount = _health / maxHealth;

        if (_health < 0)
        {
            _health = 0;
            healthText.text = "0";
            Die();
        }
    }
    public void Die()
    {
        executedEnemyCount++;

        for (int i = 0; i < expAmount; i++)
        {
            float xRandom = Random.Range(-expRangeX, expRangeX);
            float yRandom = Random.Range(-expRangeY, expRangeY);
            float zRandom = Random.Range(-expRangeZ, expRangeZ);

            Instantiate(expEffect, transform.position + new Vector3(xRandom, yRandom, zRandom), Quaternion.identity);
        }
        DestroyEnemy();
        Destroy(dieObject == null ? gameObject : dieObject);
    }
}
