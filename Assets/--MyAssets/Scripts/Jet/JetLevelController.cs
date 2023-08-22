using UnityEngine;

public class JetLevelController : MonoBehaviour
{
    public int JetLevel;
    [SerializeField] private int neededKillScoreForLevelUp;

    private void OnEnable()
    {
        EnemyVehicleHealth.DestroyEnemy += JetLevelControl;
    }
    private void OnDisable()
    {
        EnemyVehicleHealth.DestroyEnemy -= JetLevelControl;
    }

    private void Start()
    {
        UIManager.Instance.SetGeneralUIStats(JetLevel);
    }
    private void JetLevelControl()
    {
        int a = EnemyVehicleHealth.executedEnemyCount % neededKillScoreForLevelUp;
        if (a == 0)
            UIManager.Instance.UpdateExecutedEnemyTxt(1 + EnemyVehicleHealth.executedEnemyCount / neededKillScoreForLevelUp);
    }
}
