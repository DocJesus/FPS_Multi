using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    private NetworkManager manager;

    private void Start()
    {
        manager = NetworkManager.singleton;

        if (manager.matchMaker == null)
        {
            //lance le matchmaker de l'UI de base
            manager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void SetRoomSize(uint _size)
    {
        roomSize = _size;
    }

    public void CreateRoom()
    {
        if (roomName != null && roomName != "")
        {
            Debug.Log("une partie a été créer au nom " + roomName + " de taille : " + roomSize);
            //créer la partie
            manager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
        }
    }
}
