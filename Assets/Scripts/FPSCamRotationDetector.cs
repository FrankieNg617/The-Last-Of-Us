using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FPSCamRotationDetector : MonoBehaviour
{
    float lastCamRotationY;
    Decimal camRotateValue = 0;  //a number representing the range of the cam rotation
    Decimal offset = 0.005m;
    Decimal UppercamRotateValue = 3m;
    public float turning;  //variable for passing the camRotateValue to the Playmaker

    [SerializeField]
    private Camera cam;

    void Start()
    {
        lastCamRotationY = cam.gameObject.transform.rotation.y;
    }

    void Update()
    {
        if(camRotateValue < UppercamRotateValue && cam.gameObject.transform.rotation.y != lastCamRotationY)
            camRotateValue += offset;
        else if(cam.gameObject.transform.rotation.y == lastCamRotationY && camRotateValue > 0)
            camRotateValue -= offset;

        turning = (float)camRotateValue;

        lastCamRotationY = cam.gameObject.transform.rotation.y;
    }
}
