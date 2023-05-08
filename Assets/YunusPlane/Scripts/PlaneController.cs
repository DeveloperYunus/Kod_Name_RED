using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneController : MonoBehaviour
{
    [Header("Jet Physics Value")]
    [SerializeField] float m_MaxEnginePower = 40;                            // The maximum output of the engine.
    [SerializeField] float m_AerodynamicEffect = 0.02f;                      // How much aerodynamics affect the speed of the aeroplane.
    [SerializeField] float m_Lift = 0.002f;                                  // The amount of lift generated by the aeroplane moving forwards.
    [SerializeField] float m_ZeroLiftSpeed = 300;                            // The speed at which lift is no longer applied.

    [Header("Jet Physics Value")]                                            // pitchControlSensitivity, rollControlSensitivity, yawControlSensitivity
    [SerializeField] float pitchContSens;
    [SerializeField] float rollContSens;
    [SerializeField] float yawContSens;         

    float ForwardSpeed;                 // How fast the aeroplane is traveling in it's forward direction.
    float EnginePower;                  // How much power the engine is being given.
    float aeroFactor;
    float Altitude;                     // rak�m - yerden y�kseklik

    float throttleAmount, pitch, roll, yaw;
    float thTimer;                      
    Rigidbody rb;


    [Header("Drag")]
    [SerializeField]  float dragIncreaseFactor = 0.001f;        // how much drag should increase with speed.
    float originalDrag;                                         // The drag when the scene starts.
    float originalAngularDrag;                                  // The angular drag when the scene starts.


    [Header("Texts")]
    public TextMeshProUGUI throttleTxt;
    public TextMeshProUGUI speedTxt;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.drag;
        originalAngularDrag = rb.angularDrag;


        throttleAmount = 0;
        
        ForwardSpeed = 0;
        EnginePower = 0;
        aeroFactor = 0;

        throttleTxt.text = "0"+ " %";
        speedTxt.text = "0";
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        pitch = pitchContSens * Input.GetAxis("Vertical");          //W, S  --  yukar� a�a��
        roll = rollContSens * Input.GetAxis("Horizontal");        //A, D  --  kendi ekseni etraf�nda d�ner
        yaw = yawContSens * Input.GetAxis("Yaw");                   //Q, E  --  sa�, sol

        //EnginePower = m_MaxEnginePower * Input.GetAxis("Fire3");  //left shift, left alt  --  Bunu project setting > Input Manager den biz ayarlad�k
    }
    private void FixedUpdate()
    {
        if (Input.GetAxis("Fire3") > 0 && thTimer > 0 && throttleAmount < 1)
        {
            thTimer = -0.1f;
            throttleAmount += 0.01f;
        }
        else if (Input.GetAxis("Fire3") < 0 && thTimer > 0 && throttleAmount > 0)
        {
            thTimer = -0.1f;
            throttleAmount -= 0.01f;
        }
        else thTimer += Time.fixedDeltaTime;
        

        if (throttleAmount > 0)
        {
            EnginePower = throttleAmount * m_MaxEnginePower;
            throttleTxt.text = (throttleAmount * 100).ToString("0") + " %";
        }

        speedTxt.text = ForwardSpeed.ToString("0.0") + " mhp";

        //CalculateRollAndPitchAngles();                // bu kod ve 
        //AutoLevel();                                  // ve bu kodlar deaktif edildi ��nk� o�a��n kontrollerini yanl�� yans�t�yor

        CalculateForwardSpeed();

        CalculateDrag();

        CaluclateAerodynamicEffect();

        CalculateLinearForces();

        CalculateTorque();

        //CalculateAltitude();                          //�imdilik gerekmiyor
    }

    private void CalculateForwardSpeed()
    {
        // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
        var localVelocity = transform.InverseTransformDirection(rb.velocity);
        ForwardSpeed = Mathf.Max(0, localVelocity.z);
    }

    private void CalculateDrag()
    {
        // increase the drag based on speed, since a constant drag doesn't seem "Real" (tm) enough
        //float extraDrag = rb.velocity.magnitude * m_DragIncreaseFactor;
        // Air brakes work by directly modifying drag. This part is actually pretty realistic!
        rb.drag = originalDrag + rb.velocity.magnitude * dragIncreaseFactor;
        //rb.drag = AirBrakes ? (m_OriginalDrag + extraDrag) * m_AirBrakesEffect : m_OriginalDrag + extraDrag;
        // Forward speed affects angular drag - at high forward speed, it's much harder for the plane to spin
        rb.angularDrag = originalAngularDrag * ForwardSpeed;
    }

    private void CaluclateAerodynamicEffect()
    {
        // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
        // will naturally try to align itself in the direction that it's facing when moving at speed.
        // Without this, the plane would behave a bit like the asteroids spaceship!
        if (rb.velocity.magnitude > 0.1)
        {
            // compare the direction we're pointing with the direction we're moving:
            aeroFactor = Vector3.Dot(transform.forward, rb.velocity.normalized);
            // multipled by itself results in a desirable rolloff curve of the effect
            aeroFactor *= aeroFactor;
            // Finally we calculate a new velocity by bending the current velocity direction towards
            // the the direction the plane is facing, by an amount based on this aeroFactor
            var newVelocity = Vector3.Lerp(rb.velocity, transform.forward * ForwardSpeed,
                                           aeroFactor * ForwardSpeed * m_AerodynamicEffect * Time.deltaTime);
            rb.velocity = newVelocity;

            // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up pointing downwards in a stall
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity, transform.up), m_AerodynamicEffect * Time.deltaTime);
        }
    }

    private void CalculateLinearForces()
    {
        // Now calculate forces acting on the aeroplane:
        // we accumulate forces into this variable:
        var forces = Vector3.zero;
        // Add the engine power in the forward direction
        forces += EnginePower * transform.forward;
        // The direction that the lift force is applied is at right angles to the plane's velocity (usually, this is 'up'!)
        var liftDirection = Vector3.Cross(rb.velocity, transform.right).normalized;
        // The amount of lift drops off as the plane increases speed - in reality this occurs as the pilot retracts the flaps
        // shortly after takeoff, giving the plane less drag, but less lift. Because we don't simulate flaps, this is
        // a simple way of doing it automatically:
        var zeroLiftFactor = Mathf.InverseLerp(m_ZeroLiftSpeed, 0, ForwardSpeed);
        // Calculate and add the lift power
        var liftPower = ForwardSpeed * ForwardSpeed * m_Lift * zeroLiftFactor * aeroFactor;
        forces += liftPower * liftDirection;
        // Apply the calculated forces to the the Rigidbody
        rb.AddForce(forces);
    }

    private void CalculateTorque()
    {
        // We accumulate torque forces into this variable:
        var torque = Vector3.zero;
        // Add torque for the pitch based on the pitch input.
        torque += pitch * transform.right;
        // Add torque for the yaw based on the yaw input.
        torque += yaw * transform.up;
        // Add torque for the roll based on the roll input.
        torque += -roll * transform.forward;
        // Add torque for banked turning.
        // torque += m_BankedTurnAmount * m_BankedTurnEffect * transform.up;                                        //bu kod kontrollerin ger�ek�i olmas� i�in ��kart�ld�
        // The total torque is multiplied by the forward speed, so the controls have more effect at high speed,
        // and little effect at low speed, or when not moving in the direction of the nose of the plane
        // (i.e. falling while stalled)
        rb.AddTorque(ForwardSpeed * aeroFactor * torque);
    }

    private void CalculateAltitude()            //rak�m hesaplar gerekli olabilir
    {
        // Altitude calculations - we raycast downwards from the aeroplane
        // starting a safe distance below the plane to avoid colliding with any of the plane's own colliders
        var ray = new Ray(transform.position - Vector3.up * 10, -Vector3.up);
        Altitude = Physics.Raycast(ray, out RaycastHit hit) ? hit.distance + 10 : transform.position.y;
    }
}