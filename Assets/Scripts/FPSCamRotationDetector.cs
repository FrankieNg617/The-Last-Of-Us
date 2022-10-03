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

// public class FPSCamRotationDetector : MonoBehaviour 
// {
//     //Speed at which lerp should occur

//     private float speed = 1f;
//     private float rotationYDelta;

//     public float turning;

//     [SerializeField]
//     private Camera cam;

//     float lastCamRotationY;
  
//     //Lerp Direction
//     bool LerpedUp = false;

//     //Lerp time value
//     float LerpTime;

//     void Start()
//     {
//         lastCamRotationY = cam.gameObject.transform.rotation.y;
//     }
  
//     void Update()   
//     { 
//         rotationYDelta = Math.Abs(cam.gameObject.transform.rotation.y - lastCamRotationY);

      

//         //If HoldOn Right Mousebutton,Move from 0 to 1  
//         if(cam.gameObject.transform.rotation.y != lastCamRotationY) 
//         {
                     
//             LerpTime += 0.01f*(speed+rotationYDelta);
//         }
//         else if(cam.gameObject.transform.rotation.y == lastCamRotationY) 
//         {
//             LerpTime -= 0.01f*speed;
//         }  
        
//         turning = Mathf.Lerp(0 , 1, LerpTime);
//         lastCamRotationY = cam.gameObject.transform.rotation.y; 
//     } 
// }
