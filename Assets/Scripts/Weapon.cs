using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Variables

    public Gun[] loadout;
    public Transform weaponParent;

    private GameObject currentWeapon;

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
    }

    #endregion

    #region Private Methods

    void Equip(int p_ind)
    {
        if(currentWeapon != null) Destroy(currentWeapon);

        GameObject t_newEquipment = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newEquipment.transform.localPosition = Vector3.zero;
        t_newEquipment.transform.localEulerAngles = Vector3.zero;

        currentWeapon = t_newEquipment;
    }

    #endregion
}
