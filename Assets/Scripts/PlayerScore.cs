using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    private Player player;

    int lastKills = 0;
    int lastDeath = 0;

	void Start ()
    {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
	}

    //quand le joueur est supprimer un sync les données
    private void OnDestroy()
    {
        SyncNow();
    }

    //sync les données toute les x secondes
    IEnumerator SyncScoreLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            //synchronise les morts et les kills du joueur sur la base de donnée
            SyncNow();
        }
    }

    void SyncNow()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            UserAccountManager.instance.GetData(OnDataRecieved);
        }
    }

    void OnDataRecieved(string data)
    {
        if (player.kills <= lastKills && player.death <= lastDeath)
            return;

        int killsSinceLastTime = player.kills - lastKills;
        int deathSinceLastTime = player.death - lastDeath;

        //récupère les morts et les kills du joueurs dans la base de donnée
        int kills = DataTranslator.DataToKills(data);
        int death = DataTranslator.DataToDeath(data);

        int newKills = killsSinceLastTime + kills;
        int newDeath = deathSinceLastTime + death;

        string newData = DataTranslator.ValueToData(newKills, newDeath);

        lastKills = player.kills;
        lastDeath = player.death;

        Debug.Log("Syncing : " + newData);

        UserAccountManager.instance.SendData(newData);
    }
}
