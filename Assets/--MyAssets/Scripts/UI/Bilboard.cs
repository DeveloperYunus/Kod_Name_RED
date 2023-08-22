using UnityEngine;

public class Bilboard : MonoBehaviour
{
    Transform cam;

    private void Start()
    {
        cam = UIManager.Instance.Camera;
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}