using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 15;
    [SerializeField] private float hipFireDuration = 0.3f;
    [SerializeField] private Rig hipFireLayer;
    [SerializeField] private Rig bodyAimLayer;
    [SerializeField] private float bufferTimeOfHolster = 1;
    [SerializeField] private int fireRate = 10;

    private bool unHolster = false;
    private float holsterTimer;
    private float nextShootTime = 0f;

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

        if(input.x != 0 || input.y != 0 || unHolster){
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
        }

        if(Input.GetButton("Fire1") && Time.time >= nextShootTime) {
            
            nextShootTime = Time.time + 1.0f / fireRate;
            bodyAimLayer.weight = 1;
            hipFireLayer.weight = 1;
            unHolster = true;

            weapon.StartFiring();
            holsterTimer = bufferTimeOfHolster;

        } else if(unHolster) {

            weapon.StopFiring();

            if(holsterTimer > 0) {
                holsterTimer -= Time.deltaTime;
            } else {
                bodyAimLayer.weight = 0;
                hipFireLayer.weight -= Time.deltaTime / hipFireDuration;
            }

            if(hipFireLayer.weight == 0) {
                unHolster = false;
            } 
        }

        
    }


}
