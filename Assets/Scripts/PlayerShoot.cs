using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;

    // Use this for initialization
    void Start ()
    {
        weaponManager = GetComponent<WeaponManager>();

	    if (cam == null)
        {
            Debug.Log("pas de camera en référence");
            this.enabled = false;
        }
	}

    public void Update()
    {
        if (PauseMenu.isOn == true)
            return;

        //récupérer notre arme
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (currentWeapon.bullet < currentWeapon.maxBullets)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponManager.Reload();
                return;
            }
        }


        //tire semi-auto
        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            //tire automatique
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("je tireen automatique");
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate); //appeler shoot en répétant + mettre le fireRate par seconde
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot"); //pour arreter de tirer en automatique
            }
        }


    }

    /*
     * So commands are generally used to tell the server what you want to do
     * clientrpcs are used by the server to tell all clients what actually has happened.
     */

    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject inst = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(inst, 2f);
    }

    //apel unique sur le client
    [Client]
    private void Shoot()
    {
        RaycastHit _hit;

        if (!isLocalPlayer || weaponManager.isReloading)
            return;

        if (currentWeapon.bullet <= 0)
        {
            Debug.Log("Out of Bullet");
            //fct de rechargement   
            weaponManager.Reload();
            return;
        }

        currentWeapon.bullet -= 1;

        CmdOnShoot();

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.collider.tag == "Player")
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name);
            }
            CmdOnHit(_hit.point, _hit.normal);
        }

        if (currentWeapon.bullet <= 0)
        {
            Debug.Log("Out of Bullet");
            //fct de rechargement   
            weaponManager.Reload();
        }
    }

    [Command]
    private void CmdPlayerShot(string _ID, int damage, string sourceshooter)
    {
        Debug.Log(_ID + " à été touché");

        Player _player = GameManager.getPlayer(_ID);
        _player.RpcTakeDamage(damage, sourceshooter);
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //fait apparaitre les effets sur tout les joueurs
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }
}
