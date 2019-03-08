using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour
{
    public Text userNameText;

    private void Start()
    {
        if (UserAccountManager.IsLoggedIn)
            userNameText.text = UserAccountManager.LoggedIn_Username;
    }

    public void LogOut()
    {
        if (UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.LogOut();
    }
}
