using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour {

    [SerializeField]
    private Text KillerText;

    [SerializeField]
    private Text KilledText;

    public void SetUp(string player, string source)
    {
        KillerText.text = "<b>" + source + "</b>";
        KilledText.text = "<i>" + player + "</i>";
    }
}
