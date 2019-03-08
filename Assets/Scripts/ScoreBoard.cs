using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    GameObject PlayerScoreboardItem;

    [SerializeField]
    Transform PlayerScoreboardList;

    private void OnEnable()
    {
        Debug.Log("SetUp du scoreBoard");
        //récupère une array contenant tout les joueurs
        Player[] players = GameManager.getAllPLayers();
        //loop dans l'array
        foreach (Player player in players)
        {
            Debug.Log("player.name = " + player.username + " player.kills : " + player.kills + " players.death : " + player.death);
            //on instancie le nom du joueur dans le cadre (on peut mettre juste un transform)
            GameObject itemGO = (GameObject)Instantiate(PlayerScoreboardItem, PlayerScoreboardList);
            //on récupère le script
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>();

            if (item != null)
            {
                item.Setup(player.username, player.kills, player.death);
            }
        }
        //mis en pace des éléments de l'UI et les bonnes données
    }

    private void OnDisable()
    {
        //nettoyer la liste des joueurs
        foreach (Transform child in PlayerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}
