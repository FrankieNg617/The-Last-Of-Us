using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RaycastWeapon : MonoBehaviour
{


    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform raycastDestinaiton;
    [SerializeField] private TrailRenderer bulletEffect;

    private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);

    Ray ray;
    RaycastHit hitInfo;
    public bool isFiring = false;


    public void StartFiring() {
        isFiring = true;
        FireBullet();
    }

    private void FireBullet() {
        muzzleFlash.Emit(1);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestinaiton.position - raycastOrigin.position + new Vector3(
            Random.Range(-bulletSpreadVariance.x,bulletSpreadVariance.x),
            Random.Range(-bulletSpreadVariance.y,bulletSpreadVariance.y),
            Random.Range(-bulletSpreadVariance.z,bulletSpreadVariance.z));

        var tracer = Instantiate(bulletEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if(Physics.Raycast(ray, out hitInfo)) {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            tracer.transform.position = hitInfo.point;
        }

    }

    public void StopFiring() {
        isFiring = false;
    }

}
