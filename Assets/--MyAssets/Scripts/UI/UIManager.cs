using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Transform Camera;

    [SerializeField] private CrossAirController crossAirController;

    [SerializeField] private TextMeshProUGUI executedEnemyText;
    [SerializeField] private TextMeshProUGUI jetLevelText;

    private void OnEnable()
    {
        EnemyVehicleHealth.DestroyEnemy += UpdateExecutedEnemyTxt;
    }
    private void OnDisable()
    {
        EnemyVehicleHealth.DestroyEnemy -= UpdateExecutedEnemyTxt;
    }



    private void Awake()
    {
        Instance = this;
    }

    private void UpdateExecutedEnemyTxt()
    {
        executedEnemyText.text = "Executed Enemy : " + EnemyVehicleHealth.executedEnemyCount.ToString("0");
    }
    public void UpdateExecutedEnemyTxt(int jetLevel)
    {
        jetLevelText.text = "Jet Level : " + jetLevel.ToString("0");
    }

    public void SetGeneralUIStats(int jetLevel)
    {
        executedEnemyText.text = "Executed Enemy : " + EnemyVehicleHealth.executedEnemyCount.ToString("0");
        jetLevelText.text = "Jet Level : " + jetLevel.ToString("0");
    }
}
