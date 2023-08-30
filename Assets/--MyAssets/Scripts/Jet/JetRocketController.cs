using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class JetRocketController : MonoBehaviour
{
    [Header("--- Lock Stats ---")]
    [SerializeField] private float lockRayDistance;
    [SerializeField] private float checkLockControlTime;
    [SerializeField] private float lockTimer;

    private Transform cameraa;
    private bool lockState;
    private bool canRocketFollow;

    [Header("--- Rocket ---")]
    [SerializeField] private GameObject rocketObj;
    [SerializeField] private Transform rocketMuzzle;
    [SerializeField] private float rocketSpeed;

    private void Awake()
    {
        cameraa = UIManager.Instance.Camera.GetComponent<Transform>();
        lockState = false;
        canRocketFollow = false;
    }
    void Start()
    {
        InvokeRepeating(nameof(CheckAimForRocketLock), 0, checkLockControlTime);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SendRocket();
        }
    }

    private void CheckAimForRocketLock()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit lockRay, lockRayDistance);
        Debug.DrawRay(transform.position, transform.forward * lockRayDistance, Color.red, checkLockControlTime);

        if (lockRay.transform == null)
        {
            CrossAirController.Instance.SetLocketState(null, lockTimer);
            lockState = false;
            canRocketFollow = false;

            return;
        }

        var damageable = lockRay.transform.GetComponent<IDamageTakeable<float>>();
        
        if (damageable != null)
        {
            lockState = true;
            CrossAirController.Instance.SetLocketState(lockRay.transform, lockTimer);
        }

        StartCoroutine(CheckLockTime(lockTimer));
    }

    private IEnumerator CheckLockTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (lockState)
        {
            canRocketFollow = true;
        }
        else
        {
            canRocketFollow = false;
        }
    }

    public void SendRocket()
    {
        GameObject rocket = Instantiate(rocketObj, rocketMuzzle.position, rocketMuzzle.rotation);

        if (canRocketFollow)
        {            
            rocket.GetComponent<Rigidbody>().velocity = rocketMuzzle.forward * rocketSpeed;
        }
        else
        {
            rocket.GetComponent<Rigidbody>().velocity = rocketMuzzle.forward * rocketSpeed;
        }
    }
}