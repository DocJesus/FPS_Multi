using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();
    [SerializeField]
    private Text status;
    NetworkManager manager;
    [SerializeField]
    private GameObject roomListItemPrefab;
    [SerializeField]
    private Transform roomListParent;

    private void Start()
    {
        manager = NetworkManager.singleton;
        if (manager.matchMaker == null)
        {
            manager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        if (manager.matchMaker == null)
            manager.StartMatchMaker();

        manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Refreshing . . . ";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
        if (matchList == null)
        {
            status.text = "No match found";
            return;
        }

        //pour chaque game trouvé un créer un bouton et on l'assigne
        foreach(MatchInfoSnapshot match in matchList)
        {
            GameObject _roomListItem = Instantiate(roomListItemPrefab);
            _roomListItem.transform.SetParent(roomListParent);

            RoomListItem roomListItem = _roomListItem.GetComponent<RoomListItem>();
            if (roomListItem != null)
                roomListItem.SetUp(match, JoinRoom);

            _roomListItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            roomList.Add(_roomListItem);
        }

        if (roomList.Count == 0)
        {
            status.text = "No Rooms online";
        }
    }

    private void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    IEnumerator WaitForJoin()
    {
        ClearRoomList();

        int countDown = 10;
        while (countDown > 0)
        {
            status.text = "Joining . . . (" + countDown + ")";
            yield return new WaitForSeconds(1);
            countDown = - 1;
        }

        //si ce code est lu alors on a pas réussi la connection
        status.text = "fail to connect";
        yield return new WaitForSeconds(3);

        MatchInfo matchInfo = manager.matchInfo;
        if (matchInfo != null)
        {
            manager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, manager.OnDropConnection);
            manager.StopHost();
        }

        RefreshRoomList();

    }

    public void JoinRoom(MatchInfoSnapshot match)
    {
        manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);

        StartCoroutine(WaitForJoin());
    }
}
