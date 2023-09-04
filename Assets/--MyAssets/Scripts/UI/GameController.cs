using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject quitPanel;
    private bool isPaused;

    private void Start()
    {
        isPaused = false;
        quitPanel.SetActive(false);
        Time.timeScale = 1.0f;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
            {
                isPaused = false;
                quitPanel.SetActive(false);
                Time.timeScale = 1.0f;
                AudioListener.volume = 1;
            }
            else
            {
                Time.timeScale = 0f;
                isPaused = true;
                quitPanel.SetActive(true);
                AudioListener.volume = 0;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
