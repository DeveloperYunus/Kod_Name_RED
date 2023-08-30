using UnityEngine;

public class WhichJetSelected : MonoBehaviour
{
    [Tooltip("0 = KIZILELMA")]
    [SerializeField] private int defaultStarterJetID;

    private void Start()
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
                //CrossAirController.Instance.SetActiveJetTransform(transform.GetChild(i));
                return;
            }
        }


        if (id == 0)
        {
            Debug.Log(SumTwoNumber(2, 4));
        }
        else
        {
            Debug.Log(SumTwoNumber("2", "4"));
        }
    }

    void Sum()
    {
        SumTwoNumber(2, "1");
    }

    private int SumTwoNumber(int num1, int num2)
    {
        return num1 + num2;
    }

    private int SumTwoNumber(int num1, string num2)
    {
        return num1 + int.Parse(num2);
    }

    private int SumTwoNumber(string num1, string num2)
    {
        return int.Parse(num1) + int.Parse(num2);
    }
}
