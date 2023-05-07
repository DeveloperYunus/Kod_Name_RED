using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [Header("Jet Physics Value")]
    public float speed;
    public float pitchContSens, rollContSens, yawContSens;             //pitchControlSensitivity, rollControlSensitivity, yawControlSensitivity

    float throttleAmount, pitch, roll, yaw;
    float thTimer;
    bool speedUp, speedDown;                                           //true ise throttle artacak ve uçak hýzlanacak

    [Header("Texts")]
    public TextMeshProUGUI throttleTxt;
    public TextMeshProUGUI speedTxt;

    Rigidbody rb;

    //[Header("Camera Controller")]


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        throttleAmount = 0;

        throttleTxt.text = "0"+ " %";
        speedTxt.text = "0";

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            speedUp = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            speedUp = false;

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            speedDown = true;
        if (Input.GetKeyUp(KeyCode.LeftAlt))
            speedDown = false;

        pitch = pitchContSens * Input.GetAxis("Vertical");
        roll = rollContSens * Input.GetAxis("Horizontal");
        yaw = yawContSens * Input.GetAxis("Yaw");

    }
    private void FixedUpdate()
    {
        if (speedUp && thTimer > 0 && throttleAmount < 1)
        {
            thTimer = -0.05f;
            throttleAmount += 0.01f;
        }
        else if (speedDown && thTimer > 0 && throttleAmount > 0)
        {
            thTimer = -0.05f;
            throttleAmount -= 0.01f;
        }
        else thTimer += Time.fixedDeltaTime;
        

        if (throttleAmount > 0)
        {
            rb.AddForce(throttleAmount * speed * Vector3.left);
            throttleTxt.text = (throttleAmount * 100).ToString("0") + " %";
        }

        speedTxt.text = rb.velocity.ToString("0.0");
        rb.AddTorque(new(roll, -yaw, pitch));
    }
}



/*

Ctrl + Shift+ F








*/