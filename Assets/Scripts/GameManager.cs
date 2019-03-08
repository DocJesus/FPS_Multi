using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    //delegate qui va appelé plusisuers fct dans un cas de kill (killfeed)
    public delegate void OnPlayerKilledCallBack(string player, string source);
    public OnPlayerKilledCallBack OnPlayerKilled;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("double gameManager");
        }
        else
        {
            instance = this;

        }   
    }

    public void SetSceneCameraActive(bool _isActive)
    {
        if (sceneCamera == null)
        {
            return;
        }
        else
        {
            sceneCamera.SetActive(_isActive);
        }
    }

    #region PlayerTracking

    [SerializeField]
    private static string prefix = "Player ";

    //dico des joeuurs présent dans la game
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    //ajoute un joueur dans le dico
    public static void ResgisterPlayer(string _netID, Player _player)
    {
        string _playerID = prefix + _netID;
        _player.transform.name = _playerID;
        players.Add(_playerID, _player);
        
        /*
        foreach (KeyValuePair<string, Player> entry in players)
        {
            Debug.Log("entry.Value = " + entry.Value);
        }

        foreach (string tmp in players.Keys)
        {
           Debug.Log(tmp + "   -   " + players[tmp].transform.name);
        }
        */
        
    }

    //retire un joueur du dico
    public static void UnRegisterPlayer(string _player)
    {
        players.Remove(_player);
    }

    //done un joueur du dico
    public static Player getPlayer(string _playerID)
    {
        if (!players.ContainsKey(_playerID))
        {
            Debug.Log("hey! this played id is no good! " + _playerID);
            return null;
        }
        return players[_playerID];
    }

    public static Player[] getAllPLayers()
    {
        return players.Values.ToArray();
    }

    /*
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach(string _playerID in players.Keys)
        {
            GUILayout.Label(_playerID + "   -   " + players[_playerID].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    */
   
    #endregion

}
