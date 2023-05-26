using UnityEngine;

public class JetAudio : MonoBehaviour
{
    JetController jetCont;

    [Header("Jet Engine")]
    public float soundUptTime;
    public AudioSource engineAS;        //engine audio source
    public float minPitchEng, maxPitchEng;
    public AudioSource windAS;        //engine audio source
    public float minPitchWind, maxPitchWind;

    float jetSoundTimer;

    private void Start()
    {
        jetCont = GetComponentInParent<JetController>();
    }
    private void Update()
    {
        if (jetSoundTimer < 0)
        {
            jetSoundTimer = soundUptTime;

            engineAS.pitch = Mathf.Lerp(minPitchEng, maxPitchEng, jetCont.throttleAmount);
            engineAS.volume = jetCont.throttleAmount;

            windAS.pitch = Mathf.Lerp(minPitchWind, minPitchWind, jetCont.throttleAmount);
            windAS.volume = jetCont.throttleAmount;
        }
        else jetSoundTimer -= Time.deltaTime;
    }
}
