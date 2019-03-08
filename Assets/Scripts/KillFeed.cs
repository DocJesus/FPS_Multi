using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeed : MonoBehaviour {

    [SerializeField]
    private GameObject KillFeedItemPrefab;

	// Use this for initialization
	void Start ()
    {
        //abonne la fct à onplayerkilled, comme ça quand onplayerkilled est appelé (Player ligne 120) cette fct est aussi appelé
        GameManager.instance.OnPlayerKilled += OnKill;	
	}

    public void OnKill(string player, string source)
    {
        GameObject GO = (GameObject)Instantiate(KillFeedItemPrefab, this.transform);
        GO.GetComponent<KillFeedItem>().SetUp(player, source);
        Destroy(GO, 4f);
    }

}
