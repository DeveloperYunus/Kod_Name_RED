using UnityEngine;

public class WhichJetSelected : MonoBehaviour
{
    [Tooltip("0 = KIZILELMA")]
    [SerializeField] private int defaultStarterJetID;
    public static GameObject SelectedJet;

    private void OnEnable()
    {
        WhichJetSelectedd();
    }

    private void WhichJetSelectedd()
    {
        int id;
        id = SelectJetForPlay.Instance ? SelectJetForPlay.Instance.ActiveJetID : defaultStarterJetID;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<JetID>().JetIDNumber == id)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                SelectedJet = transform.GetChild(i).gameObject;
                return;
            }
        }
    }
}
