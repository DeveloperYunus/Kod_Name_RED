using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CrossAirController : MonoBehaviour
{
    public static CrossAirController Instance;
    
    [SerializeField] private Transform lockedTransform;
    [SerializeField] private Transform crossAirImage;

    private Camera cameraa;
    private float lockTimer;


    private void Awake()
    {
        Instance = this;
        cameraa = UIManager.Instance.Camera.GetComponent<Camera>();
    }
    private void FixedUpdate()
    {
        CrossAirLockState();
    }

    private void CrossAirLockState()
    {
        if (lockedTransform != null)
        {
            crossAirImage.DOMove(cameraa.WorldToScreenPoint(lockedTransform.position), lockTimer);
            crossAirImage.GetComponent<Image>().DOColor(Color.cyan, lockTimer);
        }
        else
        {
            crossAirImage.DOLocalMove(new Vector3(0, 0, 0), 0.3f);
            crossAirImage.GetComponent<Image>().DOColor(Color.white, 0.3f);
        }
    }
    public void SetLocketState(Transform lockedObject, float lockTime)
    {
        lockTimer = lockTime;
        lockedTransform = lockedObject;
    }
}






/*These for line turned into comment line because my cinemachin camera follow and look jet fighter so CrossAirImage cant follow smoothly the aimImage


private void Start()
{
    //Camera = UIManager.Instance.Camera.GetComponent<Camera>();        
}
private void Update()
{
    //Vector3 jetLookPos = Camera.WorldToScreenPoint(jetObject.position + jetObject.forward * jetlookForvardPoint);
    //crossAirImage.position = Vector3.Lerp(crossAirImage.position, jetLookPos, crossAirFollowSensitivity * Time.deltaTime);
}

public void SetActiveJetTransform(Transform jetTransform)
{
    //jetObject = jetTransform;
}

*/