using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    #region Variables

    public List<Gun> loadout;
    [HideInInspector] public Gun currentGunData;

    public Transform weaponParent;
    public LayerMask canBeShot;
    public AudioSource gunShotsfx;
    public AudioSource reloadsfx;
    public AudioSource emptyFiresfx;
    public AudioSource unholstersfx;
    public AudioSource aimingsfx;

    public AudioClip hitmarkerSound;
    public bool isAiming = false;

    private float currentCooldown;
    private int currentIndex;  //the index of current weapon
    private GameObject currentWeapon;

    private Image hitmarkerImage;
    private float hitmarkerWait;

    private bool isReloading;
    private bool isEquipping;

    private Transform muzzleFlashParent;

    private Color CLEARWHITE = new Color(1, 1, 1, 0);
    private VFX vfx;

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        vfx = GetComponent<VFX>();
        foreach (Gun a in loadout) a.Initialize();

        hitmarkerImage = GameObject.Find("HUD/Hitmarker/Image").GetComponent<Image>();
        hitmarkerImage.color = CLEARWHITE;

        if(photonView.IsMine) 
        {
            photonView.RPC("Equip", RpcTarget.AllBuffered, 0);
        }
    }

    void Update()
    {
        if (Pause.paused && photonView.IsMine) return;

        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.AllBuffered, 0); }
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha2)) { photonView.RPC("Equip", RpcTarget.AllBuffered, 1); }

        if (currentWeapon != null)
        {
            if (photonView.IsMine)
            {
                //shoot
                if (loadout[currentIndex].burst != 1)
                {
                    if (Input.GetMouseButtonDown(0) && !isReloading && !isEquipping && currentCooldown <= 0)
                    {
                        if (currentGunData.GetClip() <= 0 && currentGunData.GetStash() <= 0) 
                        {
                            //empty fire sound
                            emptyFiresfx.Stop();
                            emptyFiresfx.clip = currentGunData.emptyFireSound;
                            emptyFiresfx.Play();
                        }

                        if (loadout[currentIndex].FireBullet()) photonView.RPC("Shoot", RpcTarget.All);

                        if (currentGunData.GetClip() <= 0 && currentGunData.GetStash() > 0 && !isReloading) photonView.RPC("ReloadRPC", RpcTarget.All);
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0) && !isReloading && !isEquipping && currentCooldown <= 0)
                    {
                        if (currentGunData.GetClip() <= 0 && currentGunData.GetStash() <= 0) 
                        {
                            //empty fire sound
                            emptyFiresfx.Stop();
                            emptyFiresfx.clip = currentGunData.emptyFireSound;
                            emptyFiresfx.Play();
                        }

                        if (loadout[currentIndex].FireBullet()) photonView.RPC("Shoot", RpcTarget.All);

                        if (currentGunData.GetClip() <= 0 && currentGunData.GetStash() > 0 && !isReloading) photonView.RPC("ReloadRPC", RpcTarget.All);
                    }
                }

                //reload
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (currentGunData.GetClip() < currentGunData.clipsize && currentGunData.GetStash() > 0 && !isReloading)
                    {
                        photonView.RPC("ReloadRPC", RpcTarget.All);
                    }
                }
                  
                //cooldown
                if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
            }

            //weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
        }

        if(photonView.IsMine)
        {
            if(hitmarkerWait > 0)
            {
                hitmarkerWait -= Time.deltaTime;
            }
            else if(hitmarkerImage.color.a > 0)
            {
                hitmarkerImage.color = Color.Lerp(hitmarkerImage.color, CLEARWHITE, Time.deltaTime * 2f);
            }
        }
    }

    #endregion

    #region Private Methods

    [PunRPC]
    private void ReloadRPC()
    {
        StartCoroutine(Reload(loadout[currentIndex].reloadTime));
    }

    IEnumerator Reload(float p_wait)
    {
        isReloading = true;
        
        if (currentWeapon.GetComponent<Animator>())
            currentWeapon.GetComponent<Animator>().Play("Reload", 0 ,0);
        else
            currentWeapon.SetActive(false);

        //reload sound
        reloadsfx.Stop();
        reloadsfx.clip = currentGunData.reloadSound;
        reloadsfx.Play();

        yield return new WaitForSeconds(p_wait);

        if(currentGunData.recovery) loadout[currentIndex].SingleReload();
        else loadout[currentIndex].Reload();

        currentWeapon.SetActive(true);
        isReloading = false;
    }

    [PunRPC]
    void Equip(int p_ind)
    {
        if (loadout.Count == p_ind) return;

        if (currentWeapon != null)
        {
            if(isReloading) StopCoroutine("Reload");
            Destroy(currentWeapon);
        }

        currentIndex = p_ind;

        GameObject t_newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newWeapon.transform.localPosition = Vector3.zero;
        t_newWeapon.transform.localEulerAngles = Vector3.zero;
        t_newWeapon.GetComponent<Sway>().isMine = photonView.IsMine;

        if (photonView.IsMine) ChangeLayersRecursively(t_newWeapon, 10);
        else ChangeLayersRecursively(t_newWeapon, 0);

        currentWeapon = t_newWeapon;
        currentGunData = loadout[p_ind];

        StartCoroutine(EquipAnim());
    }

    IEnumerator EquipAnim()
    {
        isEquipping = true;

        //unhloster sound
        unholstersfx.Stop();
        unholstersfx.clip = currentGunData.unholsterSound;
        unholstersfx.Play();
        
        currentWeapon.GetComponent<Animator>().Play("Equip", 0, 0);

        yield return new WaitForSeconds(0.6f);
        isEquipping = false;
    }

    [PunRPC]
    void PickupWeapon(string name)
    {
        //find the weapon from a library
        Gun newWeapon = GunLibrary.FindGun(name);

        if(!loadout.Contains(newWeapon))
        {
            //if the pickup weapon is not in your loadout
            if (loadout.Count == 2)
            {
                //replace primary weapon 
                loadout[0] = newWeapon;
                Equip(0);
            }
            else
            {
                loadout.Add(newWeapon);
                loadout.Reverse();
                Equip(0);
            }

            newWeapon.Initialize();
        }
        else
        {
            //fill the ammo of the pickup weapon
            int newWeaponIndex = loadout.IndexOf(newWeapon);
            Equip(newWeaponIndex);
            newWeapon.Initialize();
        }
    }

    private void ChangeLayersRecursively (GameObject p_target, int p_layer)
    {
        p_target.layer = p_layer;
        foreach (Transform a in p_target.transform) ChangeLayersRecursively(a.gameObject, p_layer);
    } 

    public bool Aim(bool p_isAiming)
    {
        if (!currentWeapon) return false;

        if(isReloading) p_isAiming = false;

        isAiming = p_isAiming;
        Transform t_anchor = currentWeapon.transform.Find("Root");
        Transform t_state_ads = currentWeapon.transform.Find("States/ADS");
        Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

        if (p_isAiming)
        {
            //aim
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }
        else
        {
            //hip
            t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
        }

        return p_isAiming;
    }

    [PunRPC]
    void Shoot()
    {
        Transform t_spawn = transform.Find("Cameras/Normal Camera");   //bullet spawn point: player's camera

        //cooldown
        currentCooldown = loadout[currentIndex].fireRate;
        
        for (int i = 0; i < Mathf.Max(1, currentGunData.pellets); i++) 
        {
            //bloom
            Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();

            //muzzle flash
            muzzleFlashParent = currentWeapon.transform.Find("Root/Anchor/Resources/Muzzle Flash");
            vfx.MuzzleFlashVFX(muzzleFlashParent);

            //raycast
            RaycastHit t_hit = new RaycastHit();
            if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
            {
                if (photonView.IsMine)
                {
                    if (t_hit.collider.gameObject.layer != 11)
                    {
                        photonView.RPC("BulletImpactVFX", RpcTarget.All, t_hit.point, t_hit.normal);
                    }

                    //shooting other player on network
                    if (t_hit.collider.gameObject.layer == 11)
                    {
                        bool applyDamage = false;

                        if (GameSettings.GameMode == GameMode.FFA)
                        {
                            applyDamage = true;
                        }

                        if (GameSettings.GameMode == GameMode.TDM)
                        {
                            if (t_hit.collider.transform.root.gameObject.GetComponent<Player>().awayTeam != GameSettings.IsAwayTeam)
                            {
                                applyDamage = true;
                            }
                        }

                        if (applyDamage)
                        {
                            //give damage
                            t_hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage, PhotonNetwork.LocalPlayer.ActorNumber);

                            //show hitmarker
                            hitmarkerImage.color = Color.white;
                            gunShotsfx.PlayOneShot(hitmarkerSound);
                            hitmarkerWait = 1f;

                            //shoe blood effect
                            photonView.RPC("BloodVFX", RpcTarget.All, t_hit.point, t_hit.normal);
                        }
                    }
                }
            }
        }
        
        //sound
        gunShotsfx.Stop();
        gunShotsfx.clip = currentGunData.gunshotSound;
        gunShotsfx.pitch = 1 - currentGunData.pitchRandomization + Random.Range(-currentGunData.pitchRandomization, currentGunData.pitchRandomization);
        gunShotsfx.volume = currentGunData.shotVolume;
        gunShotsfx.Play();

        //gun fx
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickBack;
        if (currentGunData.recovery) currentWeapon.GetComponent<Animator>().Play("Recovery", 0 ,0);
    }

    [PunRPC]
    void TakeDamage(int p_damage, int p_actor)
    {
        GetComponent<Player>().TakeDamage(p_damage, p_actor);
    }

    #endregion

    #region Public Methods

    public void RefreshAmmo(Text p_text)
    {
        int t_clip = loadout[currentIndex].GetClip();
        int t_stash = loadout[currentIndex].GetStash();

        p_text.text = t_clip.ToString("D2") + " / " + t_stash.ToString("D2");
    }

    #endregion
}
