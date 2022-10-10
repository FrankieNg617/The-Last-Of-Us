using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform raycastDestinaiton;

    Ray ray;
    RaycastHit hitInfo;

    void Start() {
        muzzleFlash.SetActive(false);
        
    }

    public void StartFiring() {
        muzzleFlash.SetActive(true); 

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestinaiton.position - raycastOrigin.position;
        if(Physics.Raycast(ray, out hitInfo)) {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
        }

    }

    public void StopFiring() {
        muzzleFlash.SetActive(false); 
    }

}
