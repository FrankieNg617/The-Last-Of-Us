using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VFX : MonoBehaviour
{
    public GameObject dieVFX;
    public GameObject bloodVFX;
    public GameObject dirtVFX;
    public GameObject concreteVFX;
    public GameObject bulletHoleVFX;
    public GameObject muzzleFlashVFX;

    [PunRPC]
    public void DieVFX()
    {
        Vector3 explosionPos = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
        GameObject explosion = Instantiate(dieVFX, explosionPos, transform.rotation);
    }

    [PunRPC]
    public void BloodVFX(Vector3 hitPos, Vector3 hitNormal)
    {
        GameObject blooding = Instantiate(bloodVFX, hitPos + hitNormal * 0.001f, Quaternion.identity);
        blooding.transform.LookAt(hitPos + hitNormal);
    }

    [PunRPC]
    public void BulletImpactVFX(Vector3 hitPos, Vector3 hitNormal)
    {
        GameObject dirt = Instantiate(dirtVFX, hitPos + hitNormal * 0.001f, Quaternion.identity);
        GameObject concrete = Instantiate(concreteVFX, hitPos + hitNormal * 0.001f, Quaternion.identity);
        GameObject bulletHole = Instantiate(bulletHoleVFX, hitPos + hitNormal * 0.001f, Quaternion.identity);

        dirt.transform.LookAt(hitPos + hitNormal);
        concrete.transform.LookAt(hitPos + hitNormal);
        bulletHole.transform.LookAt(hitPos + hitNormal);
    }

    public void MuzzleFlashVFX(Transform muzzleFlashParent)
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashVFX, muzzleFlashParent.position, muzzleFlashParent.rotation, muzzleFlashParent); 
    }


}
