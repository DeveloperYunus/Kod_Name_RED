using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class JetRocketController : MonoBehaviour
{
    [Header("--- Lock Stats ---")]
    [SerializeField] private float lockRayDistance;
    [SerializeField] private float checkLockControlTime;
    [SerializeField] private float lockTime;
    private float lockTimer;

    private Transform cameraa;
    private Transform lockObject;
    private bool canRocketFollow;

    [Header("--- Rocket ---")]
    [SerializeField] private GameObject rocketObj;
    [SerializeField] private Transform rocketMuzzle;
    [SerializeField] private float rocketSpeed;
    [SerializeField] private float rocketDamage;
    [SerializeField] private float rotateSensitivity;
    [SerializeField] private float rocketLeftTime;

    private void Awake()
    {
        cameraa = UIManager.Instance.Camera.GetComponent<Transform>();
        lockObject = null;
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

        if (lockObject == null) //under this line code is for loking a object
        {
            lockTimer = lockTime;
            canRocketFollow = false;

            return;
        }
            
        if (lockTimer < 0)
        {
            CrossAirController.Instance.SetLockState(true);
            canRocketFollow = true;
        }
        else lockTimer -= Time.deltaTime;
    }

    private void CheckAimForRocketLock()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit lockRay, lockRayDistance);
        Debug.DrawRay(transform.position, transform.forward * lockRayDistance, Color.red, checkLockControlTime);

        if (lockRay.transform && lockRay.transform.TryGetComponent<IDamageTakeable<float>>(out _))
        {
            lockObject = lockRay.transform;
            CrossAirController.Instance.SetLocketTransform(lockRay.transform, lockTime);
        }
        else
        {
            CrossAirController.Instance.SetLocketTransform(null, lockTime);
            CrossAirController.Instance.SetLockState(false);

            lockObject = null;
            canRocketFollow = false;
        }
    }

    public void SendRocket()
    {
        if (GetComponent<JetController>().Altitude < 25)
            return;

        if (Mathf.Abs(transform.eulerAngles.z) < 260 && Mathf.Abs(transform.eulerAngles.z) > 100)
            return;

        GameObject rocket = Instantiate(rocketObj, rocketMuzzle.position, rocketMuzzle.rotation);
        rocket.GetComponent<Rocket>().Damage = rocketDamage;
        rocket.GetComponent<Rocket>().RocketSpeed = rocketSpeed;
        rocket.GetComponent<Rocket>().RotateSensitivity = rotateSensitivity;

        rocket.GetComponent<Rigidbody>().velocity = Vector3.up * -20 + GetComponent<Rigidbody>().velocity;

        StartCoroutine(rocket.GetComponent<Rocket>().RocketVelocity(rocketLeftTime, canRocketFollow, lockObject));
    }


}