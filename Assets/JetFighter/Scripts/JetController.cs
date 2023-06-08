using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JetController : MonoBehaviour
{
    [Header("Jet Physics Value")]
    [SerializeField] float m_MaxEnginePower = 40;               // Motorun maksimum gücünü belirler
    [SerializeField] float m_AerodynamicEffect = 0.02f;         // Aerodinemiðin uçak hýzýný ne kadar etkilediðini belirle
    [SerializeField] float m_Lift = 0.002f;                     // Uçak ileri doðrultuda giderken (forward) ne kadar "Lift" kuveeti tarafýndan etkilenir - Lift = uçaðý kaldýran kuvvet
    [SerializeField] float m_ZeroLiftSpeed = 300;               // Bu hýz sýnýrý aþýldýktan sonra Lift uygulanmaz

    [Header("Jet Physics Value")]                               
    [SerializeField] float pitchContSens;                       //W, S  --  yukarý aþaðý kontrol hassasiyeti
    [SerializeField] float rollContSens;                        //A, D  --  kendi ekseni etrafýndaki kontrol hassasiyeti
    [SerializeField] float yawContSens;                         //Q, E  --  sað, sol kontrol hassasiyeti

    float ForwardSpeed;                                         // Uçaðýn ileri doðrurultutaki (forward) toplam hýzý
    float EnginePower;                                          // Motora verilen güç
    float aeroFactor;
    float Altitude;                                             // rakým - yerden yükseklik

    [HideInInspector] public float throttleAmount;
    float pitch, roll, yaw;
    float thTimer;                      
    Rigidbody rb;


    [Header("Drag")]
    [SerializeField]  float dragIncreaseFactor = 0.001f;        // Hýz arttýkça artacak olan sürtünme miktarý
    float originalDrag;                                         // Sahne baþýndaki sürtünme miktarý
    float originalAngularDrag;                                  // Sahne baþýndaki açýsal sürtünme miktarý


    [Header("Animation")]
    public Animator anm;
    bool gearDown;

    [Header("VFX")]
    public ParticleSystem jetFirePs;
    ParticleSystem.MainModule jetFireMain;
    float jetVFXTimer;


    [Header("Texts")]
    public TextMeshProUGUI throttleTxt;
    public TextMeshProUGUI speedTxt;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jetFireMain = jetFirePs.main;

        //Sürekli güncelleneceði için uçaðýn baþlangýçtaki sürtünme miktarlarý kaydedilir
        originalDrag = rb.drag;
        originalAngularDrag = rb.angularDrag;


        throttleAmount = 0;
        
        ForwardSpeed = 0;
        EnginePower = 0;
        aeroFactor = 0;

        gearDown = true;

        throttleTxt.text = "0"+ " %";
        speedTxt.text = "0";
    }
    void Update()
    {   
        // Sahenyi resetlemek için 
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.G))
        {            
            if (!gearDown)//ekipmaný aþaðý indir
            {
                gearDown = true;
                anm.SetBool("GearDown", true);
            }
            else if (ForwardSpeed > 90) //hýzým 90 dan büyükse ekipmaný yukarý çek
            {
                gearDown = false;
                anm.SetBool("GearDown", false);
            }   
        }


        pitch = pitchContSens * Input.GetAxis("Vertical");          //W, S  --  yukarý aþaðý
        roll = rollContSens * Input.GetAxis("Horizontal");          //A, D  --  kendi ekseni etrafýnda döner
        yaw = yawContSens * Input.GetAxis("Yaw");                   //Q, E  --  sað, sol
    }
    private void FixedUpdate()
    {
        //Burasý ÖNEMLÝ!! aþaðýdaki kodun çalýþmasý için Project Settings -> Input Manager -> Fire3 -> Negative Button = left alt, Positive Button = left shift olarak ayarlanmalý
        //Uçaðýn itme kuvvetini arttýrmak yada azaltmak için kullanýlýr
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

        //Ýtme 0 dan büyükse motor etkilerini uygula
        

        if (jetVFXTimer < 0)
        {
            jetVFXTimer = 0.1f;
            jetFireMain.startLifetime = throttleAmount * 0.1f;                                              //itme kuvvetine göre motordan çýkan ateþi artýrý

            if (throttleAmount > 0)
            {
                jetFirePs.Play(false);
                //itme sýfýrdan büyükse motor gücünü günceller
                EnginePower = throttleAmount * m_MaxEnginePower;
                throttleTxt.text = (throttleAmount * 100).ToString("0") + " %";
            }
            else
            {
                jetFirePs.Stop(false);
            }
        }
        else
            jetVFXTimer -= Time.fixedDeltaTime;

        speedTxt.text = ForwardSpeed.ToString("0.0") + " km/h";

        CalculateForwardSpeed();

        CalculateDrag();

        CaluclateAerodynamicEffect();

        CalculateLinearForces();

        CalculateTorque();

        //CalculateAltitude();                          //Ýsteðe baðlý olarak kullanýlabilir
    }

    private void CalculateForwardSpeed()
    {
        // Ýleri hýz, uçaðýn ileri yöndeki hýzýdýr (notmal hýzýyla ayný deðildir)
        var localVelocity = transform.InverseTransformDirection(rb.velocity);
        ForwardSpeed = Mathf.Max(0, localVelocity.z);
    }

    private void CalculateDrag()
    {
        // Hýz artarken sürtünme kuvvetinide artýrýr. Çünkü sabit bir sürükleme "gerçekçi" deðildir.
        //float extraDrag = rb.velocity.magnitude * m_DragIncreaseFactor;
        // Hava frenleri, drag'ý (hava direncini) doðrudan deðiþtirerek çalýþýr. Normal hayatta olan bu :D
        rb.drag = originalDrag + rb.velocity.magnitude * dragIncreaseFactor;
        //rb.drag = AirBrakes ? (m_OriginalDrag + extraDrag) * m_AirBrakesEffect : m_OriginalDrag + extraDrag;
        // Ýleri hýz (ForwarSpeed), uçaðýn dönme hareketini kontrol eden açýsal (angular) direncini etkiler, yüksek ileri hýzlarda uçaðýn dönmesi çok daha zordur.
        rb.angularDrag = originalAngularDrag * ForwardSpeed;
    }

    private void CaluclateAerodynamicEffect()
    {
        // "Aerodinamik" hesaplamalar. Bu, bir uçaðýn hýzlý hareket ederken kendi yönüne doðru hizalamaya çalýþacaðý etkisinin çok basit bir yaklaþýmýdýr.
        // Bu olmazsa, uçak asteroid yada uzay gemisi gibi davranýr!
        if (rb.velocity.magnitude > 0.1)
        {
            // Yönümüzü hareket yönümüzle kýyaslar
            aeroFactor = Vector3.Dot(transform.forward, rb.velocity.normalized);
            // Kendisi ile çarpýldýðýnda, etkinin istenen azalma eðrisi elde edilir.
            aeroFactor *= aeroFactor;
            //Son olarak, mevcut hýz yönünü, uçak yönüne doðru geçiþ yaparýz. Ve mevcut hýz yönünü bu aerofactor'a baðlý olarak belirli bir miktarda yeniden hesaplarýz.
            var newVelocity = Vector3.Lerp(rb.velocity, transform.forward * ForwardSpeed,
                                           aeroFactor * ForwardSpeed * m_AerodynamicEffect * Time.deltaTime);
            rb.velocity = newVelocity;

            // Ayrýca, uçaðý hareket yönüne doðru döndürürüz - bu çok küçük bir etki olmalýdýr, ancak uçak stall durumunda aþaðýya doðru yönlenecektir.
            // Stall = Uçaðýn kanatlarýnýn ürettiði kaldýrma kuvvetinin azalmasý veya tamamen kaybolmasý sonucu meydana gelen bir uçuþ durumudur. Bu durumda uçak düþüþe geçer.
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(rb.velocity, transform.up), m_AerodynamicEffect * Time.deltaTime);
        }
    }

    private void CalculateLinearForces()
    {
        // Þimdi, uçak üzerinde etki eden kuvvetleri hesaplayalým
        // Kuvvetleri bu deðiþkende biriktiriyoruz (force)
        var forces = Vector3.zero;
        // Motor gücünü ileri yönde ekleyelim
        forces += EnginePower * transform.forward;
        // Kaldýrma kuvvetinin uygulandýðý yön, uçaðýn hýzýna dik açýlarla (bu uçaðý yukarýya kaldýran kuvvettir) uygulanýr.
        var liftDirection = Vector3.Cross(rb.velocity, transform.right).normalized;
        // Uçak hýzlandýkça, kaldýrma kuvvetinin miktarý azalýr - gerçekte bu, pilotun uçuþtan kýsa bir süre sonra flaplarý geri çekmesiyle gerçekleþir, böylece uçak daha az drag
        // (hava direnci), ancak daha az kaldýrma kuvveti üretir. Biz flaplarý simüle etmediðimiz için, bu iþlemi otomatik olarak yapmanýn basit bir yolu budur.
        var zeroLiftFactor = Mathf.InverseLerp(m_ZeroLiftSpeed, 0, ForwardSpeed);
        // Lift kuvvetini hesapla ve ekle
        var liftPower = ForwardSpeed * ForwardSpeed * m_Lift * zeroLiftFactor * aeroFactor;
        forces += liftPower * liftDirection;
        // Hesaplanan son kuvveti (force) rigidbody'ye ekle
        rb.AddForce(forces);
    }

    private void CalculateTorque()
    {
        // Tork kuvvetlerini bu deðiþkende biriktiriyoruz
        var torque = Vector3.zero;
        // Pitch giriþine baðlý olarak pitch için torku ekleyin
        torque += pitch * transform.right;
        // Yaw giriþine baðlý olarak yaw için torku ekleyin.
        torque += yaw * transform.up;
        // Roll giriþine baðlý olarak roll için torku ekleyin.
        torque += -roll * transform.forward;
        // Eðimli dönüþ için torku ekleyin -- Bu kýsmýn etkisi oynanýþ kýsmýnda beni rahatsýz ettiði için çýkarttým
        // torque += m_BankedTurnAmount * m_BankedTurnEffect * transform.up;                         
        // Toplam tork ileri hýzla çarpýlýr, böylece kontroller yüksek hýzda daha fazla etkiye sahip olur ve düþük hýzda veya uçak
        // burnunun yönünde hareket etmediðinde (yani stall durumunda düþerken) az etkiye sahiptir.
        rb.AddTorque(ForwardSpeed * aeroFactor * torque);
    }

    private void CalculateAltitude()            //rakým hesaplar gerekli olabilir
    {
        // Rakým hesaplamalarý - uçaðýn kendi collider'larýyla çarpýþmayý önlemek için güvenli bir mesafenin altýndan baþlayarak, uçaktan aþaðý doðru bir raycast gönderiyoruz.
        var ray = new Ray(transform.position - Vector3.up * 10, -Vector3.up);
        Altitude = Physics.Raycast(ray, out RaycastHit hit) ? hit.distance + 10 : transform.position.y;
    }
}