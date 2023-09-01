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

    public bool isLocked = false;


    private void Awake()
    {
        Instance = this;

        isLocked = false;
        cameraa = UIManager.Instance.Camera.GetComponent<Camera>();
    }
    private void FixedUpdate()
    {
        CrossAirLockState();
    }

    private void CrossAirLockState()
    {
        crossAirImage.DOKill();
        crossAirImage.GetComponent<Image>().DOKill();

        if (lockedTransform != null)
        {
            crossAirImage.DOMove(cameraa.WorldToScreenPoint(lockedTransform.position), lockTimer);
            crossAirImage.DOLocalMoveZ(0, 0);

            if (isLocked)
            {
                crossAirImage.GetComponent<Image>().DOColor(Color.red, 0);
                return;
            }
            crossAirImage.GetComponent<Image>().DOColor(Color.cyan, lockTimer);
        }
        else
        {
            crossAirImage.DOLocalMove(new Vector3(0, 0, 0), 0.3f);
            crossAirImage.GetComponent<Image>().DOColor(Color.white, 0.3f);
        }
    }   
    public void SetLocketTransform(Transform lockedObject, float lockTime)
    {
        lockedTransform = lockedObject;
        lockTimer = lockTime;
    }
    public void SetLockState(bool lockState)
    {
        isLocked = lockState;
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