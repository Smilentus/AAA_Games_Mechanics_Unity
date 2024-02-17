using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class NetworkPlayerColorizer : NetworkBehaviour
{
    private static NetworkPlayerColorizer instance;
    public static NetworkPlayerColorizer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NetworkPlayerColorizer>();
            }

            return instance;
        }
    }


    [field: SerializeField]
    public List<PlayerColorData> NetworkPlayerColors = new List<PlayerColorData>();

    [field: SerializeField]
    private PlayerColorData FallbackColor;


    public PlayerColorData GetPlayerColorData(int playerIndex)
    {
        if (playerIndex >= NetworkPlayerColors.Count || playerIndex < 0)
        {
            return FallbackColor;
        }
        else
        {
            return NetworkPlayerColors[playerIndex];
        }
    }
}


[System.Serializable]
public class PlayerColorData
{
    public string colorName;
    public Color color;
}