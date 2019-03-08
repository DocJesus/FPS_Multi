using UnityEngine.UI;
using UnityEngine;

public class PlayerScoreBoardItem : MonoBehaviour
{
    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text KillsText;

    [SerializeField]
    Text DeathText;

    public void Setup(string username, int kills, int death)
    {
        usernameText.text = username;
        KillsText.text = "Kills: " + kills.ToString();
        DeathText.text = "Death: " + death.ToString();
    }
}
