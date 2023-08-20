using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;
    public Transform target;

    public float rotationSpeed = 1f;
    public bool canClampAngle;                              //cameranýn y axis indeki açýlarý sýnýrlamak için

    public float minYAngle = -80f;
    public float maxYAngle = 80f;

    float xAngle = 0f;
    float yAngle = 0f;


    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    void FixedUpdate()
    {
        // get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // calculate rotation angles based on mouse movement
        xAngle += mouseX;
        yAngle += mouseY;
        if (canClampAngle) yAngle = Mathf.Clamp(yAngle, minYAngle, maxYAngle);

        // rotate target object based on angles
        Quaternion rotation = Quaternion.Euler(yAngle, xAngle, 0f);
        target.rotation = rotation;

        // move virtual camera to target position
        Vector3 cameraPosition = target.position - (rotation * Vector3.forward);
        virtualCamera.transform.position = cameraPosition;

        // make virtual camera look at target object
        virtualCamera.transform.LookAt(target);
    }
}
