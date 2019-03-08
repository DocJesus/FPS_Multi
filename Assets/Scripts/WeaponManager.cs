using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon PrimaryWeapon;
    private PlayerWeapon CurrentWeapon;

    private WeaponGraphics currentGraphics;

    public bool isReloading = false;

    private void Start()
    {
        EquipWeapon(PrimaryWeapon);
    }

    void EquipWeapon(PlayerWeapon _weap)
    {
        CurrentWeapon = _weap;

        //on instancie l'arme (de manière graphique)
        GameObject _weapIns = (GameObject)Instantiate(_weap.graphics, weaponHolder.position, weaponHolder.rotation);
        _weapIns.transform.SetParent(weaponHolder);

        //on récupère la technique de l'arme
        currentGraphics = _weapIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
        {
            Debug.LogError("pas de weaponGraphicss sur l'arme équipé: " + _weapIns.name);
        }


        if (isLocalPlayer)
        {
            //si on est sur le joueur on met un layer spécial au fligue pour la deuxième camera
            Util.SetLayerRecursiv(_weapIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return CurrentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void Reload()
    {
        if (isReloading)
        {
            return;
        }
        //coroutine pour mettre un delais sur le rechargement
        StartCoroutine(Reload_Coroutine());
    }

    public IEnumerator Reload_Coroutine()
    {

        Debug.Log("Reloading. . .");

        isReloading = true;
        CmdOnReload();

        yield return new WaitForSeconds(CurrentWeapon.reloadTime);

        CurrentWeapon.bullet = CurrentWeapon.maxBullets;

        isReloading = false;
        Debug.Log("Finished reloading");
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currentGraphics.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Reload");
        }

    }
}
