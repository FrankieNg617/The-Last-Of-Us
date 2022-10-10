using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 15;
    [SerializeField] private float hipFireDuration = 0.3f;
    [SerializeField] private Rig hipFireLayer;
    [SerializeField] private Rig bodyAimLayer;
    [SerializeField] private float bufferTimeOfHolster = 1;

    private bool isFiring = false;
    private float timer;

    Camera mainCamera;
    Vector2 input;
    RaycastWeapon weapon;
    

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        bodyAimLayer.weight = 0;
        hipFireLayer.weight = 0;    

        weapon = GetComponentInChildren<RaycastWeapon>(); 
    }

    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if(input.x != 0 || input.y != 0 || isFiring){
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
        }

        
        if(Input.GetButton("Fire1")) {
            
            bodyAimLayer.weight = 1;
            hipFireLayer.weight = 1;
            isFiring = true;

            weapon.StartFiring();
            timer = bufferTimeOfHolster;

        } else if(isFiring) {

            weapon.StopFiring();

            if(timer > 0) {
                timer -= Time.deltaTime;
            } else {
                bodyAimLayer.weight = 0;
                hipFireLayer.weight -= Time.deltaTime / hipFireDuration;
            }

            if(hipFireLayer.weight == 0) {
                isFiring = false;
            } 
        }

        
    }

}
