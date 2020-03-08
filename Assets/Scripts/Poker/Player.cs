using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Player : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public List<Card> cards = new List<Card>();
    public TexasPokerHand hand;
    public int money;
    public bool hasChosenAction = false;
    public PlayStatus playStatus;

    int totalAmountBetThisRound = 0;
    int amountToBet = 0;
    #region Properties
    public int AmountToBet { set { amountToBet = value; } }
    public int TotalBetThisRound { get { return totalAmountBetThisRound; } }
    #endregion

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private IEnumerator Start()
    {
        hand = new TexasPokerHand();
        playStatus = PlayStatus.Betting;
        //DebugShowPlayerHand();

        if (photonView.IsMine)
            PhotonGameManager.CurrentPlayer = this;

        this.name = PhotonNetwork.NickName;

        while (UIManager.instance.PlayerIconTexture == null)
            yield return null;

        SendViewIdToServer();
    }
   

    #region Player Actions
    public void Raise()
    {
        if (photonView.IsMine)
        {
            UIManager.instance.playerActionPanel.SetActive(false);
            object[] data = new object[] { amountToBet };
            RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
            SendOptions sendOptions = new SendOptions { Reliability = false };

            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerRaise, data, eventOptions, sendOptions);
        }

    }
    public void RaiseAllIn()
    {
        if (photonView.IsMine)
        {
            UIManager.instance.playerActionPanel.SetActive(false);
            object[] data = new object[] { money };
            RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
            SendOptions sendOptions = new SendOptions { Reliability = false };

            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerRaise, data, eventOptions, sendOptions);
        }

    }
    public void RaiseHalf()
    {
        if (photonView.IsMine)
        {
            amountToBet = Mathf.FloorToInt(Dealer.Pot / 2);
            if (amountToBet > money)
                amountToBet = money;
            Raise();
        }
    }
    public void RaiseQuarter()
    {
        if (photonView.IsMine)
        {
            amountToBet = Mathf.FloorToInt(Dealer.Pot / 4);
            if (amountToBet > money)
                amountToBet = money;
            Raise();
        }
    }
    public void RaiseThreeQuarters()
    {
        if (photonView.IsMine)
        {
            amountToBet = Mathf.FloorToInt(((Dealer.Pot * 3) / 4));
            if (amountToBet > money)
                amountToBet = money;
            Raise();
        }
    }
    public void Call()
    {
        if (photonView.IsMine)
        {
            UIManager.instance.playerActionPanel.SetActive(false);

            object[] data = new object[] { };
            RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
            SendOptions sendOptions = new SendOptions { Reliability = false };

            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerCall, data, eventOptions, sendOptions);
        }
    }
    public void Check()
    {
        if (photonView.IsMine)
        {
            hasChosenAction = true;
            Debug.Log(name + " checked");
            UIManager.instance.playerActionPanel.SetActive(false);

            object[] data = new object[] { };
            RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
            SendOptions sendOptions = new SendOptions { Reliability = false };

            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerCheck, data, eventOptions, sendOptions);
        }
    }
    public void Fold()
    {
        if (photonView.IsMine)
        {
            Debug.Log(name + " folded and is no longer in play");
            UIManager.instance.playerActionPanel.SetActive(false);

            object[] data = new object[] { };
            RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient };
            SendOptions sendOptions = new SendOptions { Reliability = false };

            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerFold, data, eventOptions, sendOptions);
        }
    }
    #endregion

    public void SetupHand()
    {

        cards.Sort(new CompareCardsByValue());
        //hand.SetHandStrength(cards);
        /* Debug.LogWarning(name + "'s poker hand is " + hand.strength);
         Debug.Log(name + "'s ranking card value is " + hand.rankingCard.name);
         Debug.Log(name + "'s tie breakers are: ");
         foreach (Card card in hand.tieBreakerCards) { Debug.Log(card.name);  }*/

    }

    void SendViewIdToServer()
    {
        if (photonView.IsMine)
        {
            byte[] profileImageBytes = UIManager.instance.GetReadablePlayerIconTexture().EncodeToPNG();
            object[] datas = new object[] { photonView.ViewID, PhotonNetwork.NickName, profileImageBytes };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.MasterClient
            };
            SendOptions sendOptions = new SendOptions() { Reliability = false };
            PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerViewId, datas, raiseEventOptions, sendOptions);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (photonView.IsMine)
        {
            switch (eventCode)
            {
                case (byte)EventCodes.PlayerCards:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        Debug.Log("ViewId from DEALER SERVER: " + (int)data[0]);
                        if (photonView.ViewID == (int)data[0])
                            CreateLocalPlayerCard(data);

                        if (cards.Count == 2)
                            AnimationManager.instance.PullPlayerCards();
                    }
                    break;

                case (byte)EventCodes.UpdateAllPlayerMoney:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        if (photonView.IsMine)
                        {
                            money = (int)data[0];
                            totalAmountBetThisRound = (int)data[1];
                            UIManager.instance.UpdatePlayerDisplay();
                        }
                    }
                    break;
                case (byte)EventCodes.UpdateCurrentPlayerMoney:
                    {
                        object[] data = (object[])photonEvent.CustomData;
                        int currentPlayerViewId = (int)data[0];
                        if (photonView.IsMine && photonView.ViewID == currentPlayerViewId)
                        {
                            money = (int)data[1];
                            totalAmountBetThisRound = (int)data[2];
                            UIManager.instance.UpdatePlayerDisplay();
                        }
                        break;
                    }
                case (byte)EventCodes.PlayerTurn:
                    {
                        if (photonView.IsMine)
                        {
                            object[] data = (object[])photonEvent.CustomData;
                            if (photonView.ViewID == (int)data[1])
                            {
                                Debug.Log("Player: PhotonViewId - " + (int)data[1]);
                                UIManager.instance.callBet.gameObject.SetActive((bool)data[0]);
                                UIManager.instance.ShowPlayerInterface();
                                UIManager.instance.UpdatePlayerDisplay();
                            }
                        }
                    }
                    break;
                case (byte)EventCodes.ClearPlayerCards:
                    {
                        cards.Clear();
                        break;
                    }
                case (byte)EventCodes.GrantWinnerMoney:
                    {
                        if (photonView.IsMine)
                        {
                            object[] datas = (object[])photonEvent.CustomData;
                            int[] winnerIds = (int[])datas[0];
                            if (winnerIds.Contains(photonView.ViewID))
                            {
                                money += (int)datas[1] / winnerIds.Length;
                                UIManager.instance.UpdatePlayerDisplay();
                                AudioManager.PlayVictory();
                            }
                        }
                    }
                    break;
                case (byte)EventCodes.ServerDisconnected:
                    {
                        if (photonView.IsMine)
                        {
                            Debug.Log("Disconnected from server event");
                            object[] datas = (object[])photonEvent.CustomData;
                            PhotonNetwork.LeaveRoom();
                            PhotonNetwork.LoadLevel(0);
                        }
                    }
                    break;
            }
        }

        /*if (eventCode == (byte)EventCodes.PlayerCards && photonView.IsMine)
        //{
        //    object[] data = (object[])photonEvent.CustomData;
        //    CreateLocalPlayerCard(data);
        //}

        //if(eventCode == (byte)EventCodesUpdateAllPlayerMoney)
        //{
        //    object[] data = (object[])photonEvent.CustomData;
        //    money += (int)data[0];
        //    UIManager.instance.UpdatePlayerDisplay();
        //}

        //if(eventCode == (byte)EventCodes.PlayerTurn)
        //{
        //    object[] data = (object[])photonEvent.CustomData;
        //    UIManager.instance.callBet.gameObject.SetActive((bool)data[0]);
        //}*/
    }

    void CreateLocalPlayerCard(object[] data)
    {
        Card newCard;
        CardValue value = (CardValue)data[1];
        newCard = ScriptableObject.CreateInstance<Card>();
        newCard.name = value + " of " + (CardSuit)data[2];
        newCard.value = value;
        newCard.suit = (CardSuit)data[2];
        Dealer.SetCardSprite(newCard);
        cards.Add(newCard);
        Debug.Log("Player " + name + " Recieved card " + (CardValue)data[1] + " of " + (CardSuit)data[2]);
        UIManager.instance.UpdatePlayerDisplay();
    }

    /*
   public void Draw()
   {
       Card card = Dealer.Pull();
       cards.Add(card);
   }
   public void Discard()
   {
       cards = cards.Where(card => !card.markedForDiscard).ToList();

       while (cards.Count < 5)
       {
           Draw();
       }

       SetupHand();
       hasChosenAction = true;
   }
   */
}
