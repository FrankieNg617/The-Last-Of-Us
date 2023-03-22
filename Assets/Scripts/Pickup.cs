using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pickup : MonoBehaviourPunCallbacks
{
    public Gun[] weapons;
    public float cooldown;
    public GameObject gunDisplay;
    public List<GameObject> targets;

    private Gun weapon;  //current pickup weapon
    private GameObject newDisplay;  //current pickup weapon's display 
    private bool isDisabled;
    private float wait;

    private void Start()
    {
        foreach (Transform t in gunDisplay.transform) Destroy(t.gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SpawnWeapon", RpcTarget.AllBuffered, 0);
        }
        
    }

    private async void Update()
    {   
        if (isDisabled)
        {
            if (wait >= 0)
            {
                wait -= Time.deltaTime;
            }
            else
            {
                Enable();
            } 
        }    
    }

    [PunRPC]
    private void SpawnWeapon(int P_ind)
    {
        weapon = weapons[P_ind];
        newDisplay = Instantiate(weapon.display, gunDisplay.transform.position, gunDisplay.transform.rotation) as GameObject;
        newDisplay.transform.SetParent(gunDisplay.transform);

        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().Play("Spawn", 0, 0);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.gameObject.tag.Equals("Player"))
        {
            Weapon weaponController = other.attachedRigidbody.gameObject.GetComponent<Weapon>();
            weaponController.photonView.RPC("PickupWeapon", RpcTarget.AllBuffered, weapon.name);
            photonView.RPC("Disable", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void Disable ()
    {
        isDisabled = true;
        wait = cooldown;

        foreach (GameObject a in targets) a.SetActive(false);
        Destroy(newDisplay);
        GetComponent<Animator>().enabled = false;
    } 

    private void Enable ()
    {
        isDisabled = false;
        wait = 0;

        foreach (GameObject a in targets) a.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            int nextWeaponIndex = Random.Range(0, weapons.Length);
            photonView.RPC("SpawnWeapon", RpcTarget.AllBuffered, nextWeaponIndex);
        }
    } 
}
