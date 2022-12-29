using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private CinemachineFreeLook tpsCam;

    [SerializeField]
    private CinemachineVirtualCamera fpsCam;

    [SerializeField]
    private GameObject fpsArm;

    private bool isFPS = false;

    void Update() {
        if(Input.GetButtonDown("ChangeCamera")) {
            SwitchCamera();
        }

    }

    private void SwitchCamera() {
        if(isFPS) {
            fpsCam.Priority = 0;
            tpsCam.Priority = 1;

            //resume the movement of fps arm 
            fpsArm.GetComponent<FPSCharacterMovement>().enabled = false;
        }
        else {
            fpsCam.Priority = 1;
            tpsCam.Priority = 0;

            //stop the movement of fps arm
            fpsArm.GetComponent<FPSCharacterMovement>().enabled = true;
        }

        isFPS = !isFPS;
    }
}
