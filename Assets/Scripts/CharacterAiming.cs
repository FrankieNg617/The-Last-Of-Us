using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    public float turnSpeed = 15;
    public float hipFireDuration = 0.3f;
    public Rig hipFireLayer;

    Camera mainCamera;
    Vector2 input;
    

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;       
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        // if(input.x > 0 || input.y > 0){
        //     float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
        // }

        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
        
        if(Input.GetMouseButton(0)) {
            hipFireLayer.weight += Time.deltaTime / hipFireDuration;
        } else {
            hipFireLayer.weight -= Time.deltaTime / hipFireDuration;
        }
    }
}
