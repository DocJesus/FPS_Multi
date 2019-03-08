using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;


public class PauseMenu : MonoBehaviour
{
    private NetworkManager manager;
    public static bool isOn = false;

    private void Start()
    {
        manager = NetworkManager.singleton;
    }

    public void LeaveRoom()
    {
        MatchInfo matchInfo = manager.matchInfo;
        manager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, manager.OnDropConnection);
        manager.StopHost();
    }

}
