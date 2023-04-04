using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VFX : MonoBehaviour
{
    public GameObject dieVFX;
    public GameObject bloodVFX;

    [PunRPC]
    public void DieVFX()
    {
        Vector3 explosionPos = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
        GameObject explosion = Instantiate(dieVFX, explosionPos, transform.rotation);
    }

    [PunRPC]
    public void BloodVFX(Vector3 bloodPos)
    {
        GameObject blooding = Instantiate(bloodVFX, bloodPos, Quaternion.identity);
    }


}
