using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using Photon.Realtime;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    public static event DealingCardsEvent OnDealingCards;
    public static PhotonGameManager instance;
    public static List<Player> players;

    //static Player currentPlayer;

    public static Player CurrentPlayer { get; set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void FindPlayers()
    {
        if (players == null)
            players = new List<Player>();

        players.Clear();
        players = FindObjectsOfType<Player>().ToList();
        CurrentPlayer = players[0];
    }

    

}
