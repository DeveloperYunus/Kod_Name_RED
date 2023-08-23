using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectJetForPlay : MonoBehaviour
{
    public static SelectJetForPlay Instance;
    [HideInInspector] public int ActiveJetID;

    [SerializeField] private Transform jetHolder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        ActiveJetID = 0;                                                    //default = kýzýlelma
    }

    public void SelectJetMode(GameObject modelObject)
    {
        for (int i = 0; i < jetHolder.childCount; i++)
        {
            jetHolder.GetChild(i).gameObject.SetActive(false);
        }
        modelObject.SetActive(true);

        ActiveJetID = modelObject.GetComponent<JetID>().JetIDNumber;
    }
    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
