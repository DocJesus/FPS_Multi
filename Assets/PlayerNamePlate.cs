using UnityEngine;
using UnityEngine.UI;

public class PlayerNamePlate : MonoBehaviour {

    [SerializeField]
    private Text userNameText;

    [SerializeField]
    private Player player;
	
	// Update is called once per frame
	void Update ()
    {
        userNameText.text = player.username;	
	}
}
