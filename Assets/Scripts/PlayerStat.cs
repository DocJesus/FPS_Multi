using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour {

    public Text KillCount;
    public Text DeathCount;

	// Use this for initialization
	void Start ()
    {
        if (UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.GetData(OnReceivedData);
	}

    public void OnReceivedData(string data)
    {
        KillCount.text = DataTranslator.DataToKills(data).ToString();
        DeathCount.text = DataTranslator.DataToDeath(data).ToString();

    }

}
