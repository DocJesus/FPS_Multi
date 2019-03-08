using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class Player : NetworkBehaviour
{
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public float GetHealthPCT()
    {
        return (float)currentHealth / maxHealth;
    }

    [SyncVar]
    public string username = "Loading . . .";

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectOnDeath;

    private bool[] wasEnabled;

    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetUp = true;

    public int kills;
    public int death;

    public void SetUp()
    {

        CmdBroadCastNewPlayerSetUp();

        if (isLocalPlayer)
        {
            //désactivation de la camera de la scene
            GameManager.instance.SetSceneCameraActive(false);
            //réativer l'UI
            GetComponent<PlayerController>().playerUIInstance.SetActive(true);
        }


    }

    [Command]
    private void CmdBroadCastNewPlayerSetUp()
    {
            RpcSetUpPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetUpPlayerOnAllClients()
    {
        //pour savoir quel component était enable au lancement du joueur
        if (firstSetUp)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetUp = false;
        }

        SetDefault();
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(999, null);
        }
    }

    [ClientRpc] //RPC pour dire que le serveur appel cette méthode depuis le client
    public void RpcTakeDamage(int amount, string sourceID)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        //Debug.Log(transform.name + " a maintenant " + currentHealth + " points de vie");

        if (currentHealth <= 0)
        {
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;
        death += 1;
        Player sourcePlayer = GameManager.getPlayer(sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.kills += 1;
            GameManager.instance.OnPlayerKilled.Invoke(username, sourcePlayer.username);
        }

        //désactiver les component
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //désactive les gameObjects du joueur
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(false);
        }

        Debug.Log(transform.name + " est mort");

        //désactive le collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        //joue les particules de mort   
        GameObject particuleDeath = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(particuleDeath, 3f);

        //changement de camera
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            //désactiver l'UI
            //GetComponent<PlayerController>().playerUIInstance.SetActive(false);
        }

        //appel fonction respawn
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        Transform _startPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _startPoint.position;
        transform.rotation = _startPoint.rotation;
        yield return new WaitForSeconds(0.1f);
        SetUp();
    }

    //remet au valeurs de base
    private void SetDefault()
    {
        isDead = false;
        currentHealth = maxHealth;

        //réactive les behavior
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //réactive les gameObjects du joueur
        for (int i = 0; i < disableGameObjectOnDeath.Length; i++)
        {
            disableGameObjectOnDeath[i].SetActive(true);
        }

        //active son collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        //joue les particules de respawn
        GameObject particuleSpawn = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(particuleSpawn, 3f);
    }
}
