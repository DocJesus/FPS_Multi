using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "glock";
    public int damage = 10;
    public float range = 100f;
    public float reloadTime = 1;

    public float fireRate = 1f;

    public int maxBullets = 20;
    [HideInInspector]
    public int bullet; //nombre de balles actuel

    public GameObject graphics;

    public PlayerWeapon()
    {
        bullet = maxBullets;
    }

}
