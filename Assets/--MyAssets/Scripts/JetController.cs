using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JetController : MonoBehaviour
{
    [Header("--- Jet Physics Value ---")]
    [SerializeField] float m_MaxEnginePower = 40;               // Motorun maksimum g�c�n� belirler
    [SerializeField] float m_AerodynamicEffect = 0.02f;         // Aerodinemi�in u�ak h�z�n� ne kadar etkiledi�ini belirle
    [SerializeField] float m_Lift = 0.002f;                     // U�ak ileri do�rultuda giderken (forward) ne kadar "Lift" kuveeti taraf�ndan etkilenir - Lift = u�a�� kald�ran kuvvet
    [SerializeField] float m_ZeroLiftSpeed = 300;               // Bu h�z s�n�r� a��ld�ktan sonra Lift uygulanmaz

    [Header("--- Jet Control Value ---")]                               
    [SerializeField] float pitchContSens;                       //W, S  --  yukar� a�a�� kontrol hassasiyeti
    [SerializeField] float rollContSens;                        //A, D  --  kendi ekseni etraf�ndaki kontrol hassasiyeti
    [SerializeField] float yawContSens;                         //Q, E  --  sa�, sol kontrol hassasiyeti

    float ForwardSpeed;                                         // U�a��n ileri do�rurultutaki (forward) toplam h�z�
    float EnginePower;                                          // Motora verilen g��
    float aeroFactor;
    float Altitude;                                             // rak�m - yerden y�kseklik

    [HideInInspector] public float throttleAmount;
    float pitch, roll, yaw;
    float thTimer;                      
    Rigidbody rb;

    [Header("--- Drag ---")]
    [SerializeField]  float dragIncreaseFactor = 0.001f;        // H�z artt�k�a artacak olan s�rt�nme miktar�
    float originalDrag;                                         // Sahne ba��ndaki s�rt�nme miktar�
    float originalAngularDrag;                                  // Sahne ba��ndaki a��sal s�rt�nme miktar�


    [Header("--- Gear ---")]
    [SerializeField] private Transform gear;
    bool gearOpen;
    float jetVFXTimer;


    [Header("--- Texts ---")]
    public TextMeshProUGUI throttleTxt;
    public TextMeshProUGUI speedTxt;


    //---------------   Events  -------------------

    public delegate void EngineVFXPlay(bool vfxPlay, bool vfxChildControl);
    public static event EngineVFXPlay JetEngineVfx;

    public delegate void OneFloat(float value);
    public static event OneFloat EngineVFXStartTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //S�rekli g�ncellenece�i i�in u�a��n ba�lang��taki s�rt�nme miktarlar� kaydedilir
        originalDrag = rb.drag;
        originalAngularDrag = rb.angularDrag;


        throttleAmount = 0;
        
        ForwardSpeed = 0;
        EnginePower = 0;
        aeroFactor = 0;
        jetVFXTimer = -1;

        gearOpen = true;

        throttleTxt.text = "0"+ " %";
        speedTxt.text = "0";
    }
    void Update()
    {   
        // Sahenyi resetlemek i�in 
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.G))
        {            
            if (!gearOpen)
            {
                gearOpen = true;

                gear.DOKill();
                gear.DOScaleY(1, 1f);
            }
            else if (ForwardSpeed > 90) //h�z�m 90 dan b�y�kse ekipman� yukar� �ek
            {
                gearOpen = false;

                gear.DOKill();
                gear.DOScaleY(0, 1f);
            }   
        }


        pitch = pitchContSens * Input.GetAxis("Vertical");          //W, S  --  yukar� a�a��
        roll = rollContSens * Input.GetAxis("Horizontal");          //A, D  --  kendi ekseni etraf�nda d�ner
        yaw = yawContSens * Input.GetAxis("Yaw");                   //Q, E  --  sa�, sol
    }
    private void FixedUpdate()
    {
        //Buras� �NEML�!! a�a��daki kodun �al��mas� i�in Project Settings -> Input Manager -> Fire3 -> Negative Button = left alt, Positive Button = left shift olarak ayarlanmal�
        //U�a��n itme kuvvetini artt�rmak yada azaltmak i�in kullan�l�r
        if (Input.GetAxis("Fire3") > 0 && thTimer > 0 && throttleAmount < 1)
        {
            thTimer = -0.08f;
            throttleAmount += 0.01f;
        }
        else if (Input.GetAxis("Fire3") < 0 && thTimer > 0 && throttleAmount > 0)
        {
            thTimer = -0.08f;
            throttleAmount -= 0.01f;
        }
        else thTimer += Time.fixedDeltaTime;

        //�tme 0 dan b�y�kse motor etkilerini uygula
        

        if (jetVFXTimer < 0)
        {
            jetVFXTimer = 0.1f;
            EngineVFXStartTime?.Invoke(throttleAmount * 0.1f);

            if (throttleAmount > 0)
            {
                JetEngineVfx?.Invoke(true, false);

                //itme s�f�rdan b�y�kse motor g�c�n� g�nceller
                EnginePower = throttleAmount * m_MaxEnginePower;

                throttleTxt.text = (throttleAmount * 100).ToString("0") + " %";
                speedTxt.text = ForwardSpeed.ToString("0.0") + " km/h";
            }
            else
            {
                JetEngineVfx?.Invoke(false, false);
                speedTxt.text = "0.0" + " km/h";
            }
        }
        else
            jetVFXTimer -= Time.fixedDeltaTime;

        

        CalculateForwardSpeed();

        CalculateDrag();

        CaluclateAerodynamicEffect();

        CalculateLinearForces();

        CalculateTorque();

        //CalculateAltitude();                          //�ste�e ba�l� olarak kullan�labilir
    }

    private void CalculateForwardSpeed()
    {
        // �leri h�z, u�a��n ileri y�ndeki h�z�d�r (notmal h�z�yla ayn� de�ildir)
        var localVelocity = transform.InverseTransformDirection(rb.velocity);
        ForwardSpeed = Mathf.Max(0, localVelocity.z);
    }

    private void CalculateDrag()
    {
        // H�z artarken s�rt�nme kuvvetinide art�r�r. ��nk� sabit bir s�r�kleme "ger�ek�i" de�ildir.
        //float extraDrag = rb.velocity.magnitude * m_DragIncreaseFactor;
        // Hava frenleri, drag'� (hava direncini) do�rudan de�i�tirerek �al���r. Normal hayatta olan bu :D
        rb.drag = originalDrag + rb.velocity.magnitude * dragIncreaseFactor;
        //rb.drag = AirBrakes ? (m_OriginalDrag + extraDrag) * m_AirBrakesEffect : m_OriginalDrag + extraDrag;
        // �leri h�z (ForwarSpeed), u�a��n d�nme hareketini kontrol eden a��sal (angular) direncini etkiler, y�ksek ileri h�zlarda u�a��n d�nmesi �ok daha zordur.
        rb.angularDrag = originalAngularDrag * ForwardSpeed;
    }

    private void CaluclateAerodynamicEffect()
    {
        // "Aerodinamik" hesaplamalar. Bu, bir u�a��n h�zl� hareket ederken kendi y�n�ne do�ru hizalamaya �al��aca�� etkisinin �ok basit bir yakla��m�d�r.
        // Bu olmazsa, u�ak asteroid yada uzay gemisi gibi davran�r!
        if (rb.velocity.magnitude > 0.1)
        {
            // Y�n�m�z� hareket y�n�m�zle k�yaslar
            aeroFactor = Vector3.Dot(transform.forward, rb.velocity.normalized);
            // Kendisi ile �arp�ld���nda, etkinin istenen azalma e�risi elde edilir.
            aeroFactor *= aeroFactor;
            //Son olarak, mevcut h�z y�n�n�, u�ak y�n�ne do�ru ge�i� yapar�z. Ve mevcut h�z y�n�n� bu aerofactor'a ba�l� olarak belirli bir miktarda yeniden hesaplar�z.
            var newVelocity = Vector3.Lerp(rb.velocity, transform.forward * ForwardSpeed,
                                           aeroFactor * ForwardSpeed * m_AerodynamicEffect * Time.deltaTime);
            rb.velocity = newVelocity;

            // Ayr�ca, u�a�� hareket y�n�ne do�ru d�nd�r�r�z - bu �ok k���k bir etki olmal�d�r, ancak u�ak stall durumunda a�a��ya do�ru y�nlenecektir.
            // Stall = U�a��n kanatlar�n�n �retti�i kald�rma kuvvetinin azalmas� veya tamamen kaybolmas� sonucu meydana gelen bir u�u� durumudur. Bu durumda u�ak d����e ge�er.
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity, transform.up), m_AerodynamicEffect * Time.deltaTime);
        }
    }

    private void CalculateLinearForces()
    {
        // �imdi, u�ak �zerinde etki eden kuvvetleri hesaplayal�m
        // Kuvvetleri bu de�i�kende biriktiriyoruz (force)
        var forces = Vector3.zero;
        // Motor g�c�n� ileri y�nde ekleyelim
        forces += EnginePower * transform.forward;
        // Kald�rma kuvvetinin uyguland��� y�n, u�a��n h�z�na dik a��larla (bu u�a�� yukar�ya kald�ran kuvvettir) uygulan�r.
        var liftDirection = Vector3.Cross(rb.velocity, transform.right).normalized;
        // U�ak h�zland�k�a, kald�rma kuvvetinin miktar� azal�r - ger�ekte bu, pilotun u�u�tan k�sa bir s�re sonra flaplar� geri �ekmesiyle ger�ekle�ir, b�ylece u�ak daha az drag
        // (hava direnci), ancak daha az kald�rma kuvveti �retir. Biz flaplar� sim�le etmedi�imiz i�in, bu i�lemi otomatik olarak yapman�n basit bir yolu budur.
        var zeroLiftFactor = Mathf.InverseLerp(m_ZeroLiftSpeed, 0, ForwardSpeed);
        // Lift kuvvetini hesapla ve ekle
        var liftPower = ForwardSpeed * ForwardSpeed * m_Lift * zeroLiftFactor * aeroFactor;
        forces += liftPower * liftDirection;
        // Hesaplanan son kuvveti (force) rigidbody'ye ekle
        rb.AddForce(forces);
    }

    private void CalculateTorque()
    {
        // Tork kuvvetlerini bu de�i�kende biriktiriyoruz
        var torque = Vector3.zero;
        // Pitch giri�ine ba�l� olarak pitch i�in torku ekleyin
        torque += pitch * transform.right;
        // Yaw giri�ine ba�l� olarak yaw i�in torku ekleyin.
        torque += yaw * transform.up;
        // Roll giri�ine ba�l� olarak roll i�in torku ekleyin.
        torque += -roll * transform.forward;
        // E�imli d�n�� i�in torku ekleyin -- Bu k�sm�n etkisi oynan�� k�sm�nda beni rahats�z etti�i i�in ��kartt�m
        // torque += m_BankedTurnAmount * m_BankedTurnEffect * transform.up;                         
        // Toplam tork ileri h�zla �arp�l�r, b�ylece kontroller y�ksek h�zda daha fazla etkiye sahip olur ve d���k h�zda veya u�ak
        // burnunun y�n�nde hareket etmedi�inde (yani stall durumunda d��erken) az etkiye sahiptir.
        rb.AddTorque(ForwardSpeed * aeroFactor * torque);
    }

    private void CalculateAltitude()            //rak�m hesaplar gerekli olabilir
    {
        // Rak�m hesaplamalar� - u�a��n kendi collider'lar�yla �arp��may� �nlemek i�in g�venli bir mesafenin alt�ndan ba�layarak, u�aktan a�a�� do�ru bir raycast g�nderiyoruz.
        var ray = new Ray(transform.position - Vector3.up * 10, -Vector3.up);
        Altitude = Physics.Raycast(ray, out RaycastHit hit) ? hit.distance + 10 : transform.position.y;
    }
}