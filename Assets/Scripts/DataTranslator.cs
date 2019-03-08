using System;
using UnityEngine;

public class DataTranslator : MonoBehaviour
{
    private static string separatorKills = "[KILLS]";
    private static string separatorDeath = "[DEATH]";

    //renvoie une string dans le bon forma avec les paramètres passés
    public static string ValueToData(int kills, int death)
    {
        string tmp;

        tmp = separatorKills + kills + ";" + separatorDeath + death;

        return tmp;
    }

    //retoure la valeur après separatorKills
    public static int DataToKills(string Data)
    {
        string tmp;

        tmp = DataValue(Data, separatorKills);

        return int.Parse(tmp);
    }

    //retourne la valeur après separatorDeath
    public static int DataToDeath(string Data)
    {
        string tmp;

        tmp = DataValue(Data, separatorDeath);

        return int.Parse(tmp);
    }

    //parseur
    private static string DataValue(string Data, string symbol)
    {
        string[] array = Data.Split(';');

        foreach (string tmp in array)
        {
            if (tmp.StartsWith(symbol))
            {
                return tmp.Substring(symbol.Length);
            }
        }

        Debug.LogError(symbol + " not found " + Data);
        return null;
    }

}
