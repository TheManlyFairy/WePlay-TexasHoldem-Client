using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class UIManager : MonoBehaviour
{
    public Text playerName;
    public Image playerIcon;
    public PlayerHandDisplay playerHandDisplay;
    // public CommunityHandDisplay communityHandDisplay;
    public Text playerMoney;
    public Text playerCurrentBet;
    // public Text currentPot;
    public Slider raiseBetSlider;
    // public InputField betValueField;
    public Button raiseBet;
    public Button callBet;
    public Button check;
    public Button fold;
    public GameObject playerActionPanel;

    public static UIManager instance;

    Texture2D playerIconTexture;
    public Texture2D PlayerIconTexture { get { return playerIconTexture; } }
    private void Awake()
    {
        instance = this;
        LoginManager loginManager = FindObjectOfType<LoginManager>();
        if (loginManager && loginManager.loginMethod != LoginManager.LoginMethod.Guest)
            StartCoroutine(GenerateProfilePicture(loginManager));
        else
            playerIconTexture = playerIcon.sprite.texture;
    }
    private void Start()
    {
        raiseBetSlider.onValueChanged.AddListener(delegate { UpdateRaiseSlider(); });
        playerActionPanel.SetActive(false);
        playerName.text = Photon.Pun.PhotonNetwork.NickName;
    }
    public void QuitGame()
    {
        StartCoroutine("QuitGameCor");
    }

    IEnumerator QuitGameCor()
    {
        PhotonGameManager.CurrentPlayer.SendDisconnectToServer();
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

    public static void StartGame()
    {
        //instance.SetupUIListeners();
        PhotonGameManager.OnDealingCards += instance.UpdatePlayerDisplay;

    }
    public void ShowPlayerInterface()
    {
        playerActionPanel.SetActive(true);
        raiseBetSlider.value = 0;
        UpdateRaiseSlider();
        playerCurrentBet.text = Dealer.HighestBetMade - PhotonGameManager.CurrentPlayer.TotalBetThisRound + Dealer.MinimumBet + " $";
    }
    public void UpdatePlayerDisplay()
    {
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.CurrentPlayer);
        playerName.text = PhotonGameManager.CurrentPlayer.name;
        playerMoney.text = string.Format("Cash: {0:n0}$", PhotonGameManager.CurrentPlayer.money);

        if (PhotonGameManager.CurrentPlayer.TotalBetThisRound < Dealer.HighestBetMade)
        {
            callBet.gameObject.SetActive(true);
            check.gameObject.SetActive(false);
        }
        else
        {
            callBet.gameObject.SetActive(false);
            check.gameObject.SetActive(true);
        }
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.CurrentPlayer);
        // playerName.text = PhotonGameManager.CurrentPlayer.name;
    }
    
    void UpdateRaiseSlider()
    {
        int sliderMinimum = Dealer.HighestBetMade - PhotonGameManager.CurrentPlayer.TotalBetThisRound + Dealer.MinimumBet;
        int sliderMaximum = PhotonGameManager.CurrentPlayer.money;
        int betValue = (int)((raiseBetSlider.value * (sliderMaximum - sliderMinimum) + sliderMinimum));
        betValue -= (betValue % Dealer.MinimumBet);

        PhotonGameManager.CurrentPlayer.AmountToBet = betValue;
        playerCurrentBet.text = string.Format("{0:n0}$", betValue);
        //betValueField.text = "" + betValue;
    }
    void UpdateGameInterface()
    {
        playerMoney.text = string.Format("Cash: {0:n0}$", PhotonGameManager.CurrentPlayer.money);
        //  currentPot.text = "Total Cash Prize: " + Dealer.Pot;
    }

    IEnumerator GenerateProfilePicture(LoginManager manager)
    {
        while (Social.Active.localUser.image == null)
        {
            yield return null;
        }
        playerIconTexture = Social.Active.localUser.image;
        playerIcon.sprite = Sprite.Create(playerIconTexture, new Rect(0, 0, playerIconTexture.width, playerIconTexture.height), Vector2.zero);
    }

    public Texture2D GetReadablePlayerIconTexture()
    {
        RenderTexture tmp = RenderTexture.GetTemporary(playerIconTexture.width, playerIconTexture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(playerIconTexture, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;
        Texture2D readableTexture = new Texture2D(playerIconTexture.width, playerIconTexture.height);
        readableTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTexture.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);
        return readableTexture;
    }



    /* Disabled Code
     * public void DebugShowPlayer(int index)
    {
        playerMoney.text = "Cash: " + PhotonGameManager.players[index].money;
        // currentPot.text = "Total Cash Prize: " + Dealer.Pot;
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
        playerHandDisplay.SetupPlayerHand(PhotonGameManager.players[index]);
        playerName.text = PhotonGameManager.players[index].name;
    }
     * void Update()
    {
      if(betValueSlider.value != )
       {
            UpdateBet();
        }
    }
   void SetupUIListeners()
   {
       PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
       Dealer.OnInterfaceUpdate += UpdateGameInterface;
       Dealer.OnInterfaceUpdate += UpdatePlayerDisplay;
       betValueSlider.onValueChanged.AddListener(delegate
       {
           int sliderMinimum = Dealer.MinimumBet;
           int sliderMaximum = PhotonGameManager.CurrentPlayer.money;
           int betValue = (int)(betValueSlider.value * sliderMaximum);
           Debug.Log("Player's Bet: " + betValue);

           if (betValue >= Dealer.MinimumBet)
           {
               while (betValue % Dealer.MinimumBet != 0)
                   betValue--;
           }
           else
           {
               betValue = Dealer.MinimumBet;
           }
           PhotonGameManager.CurrentPlayer.AmountToBet = betValue;
           betValueField.text = "" + betValue;
       });
       raiseBet.onClick.AddListener(delegate
       {
           PhotonGameManager.CurrentPlayer.Raise();
           betValueSlider.value = 0;
       });
       callBet.onClick.AddListener(delegate
       {
           PhotonGameManager.CurrentPlayer.Call();
           betValueSlider.value = 0;
       });
       check.onClick.AddListener(delegate
       {
           PhotonGameManager.CurrentPlayer.Check();
           betValueSlider.value = 0;
       });
       fold.onClick.AddListener(delegate
       {
           PhotonGameManager.CurrentPlayer.Fold();
           betValueSlider.value = 0;
       });
       PhotonGameManager.OnDealingCards += UpdatePlayerDisplay;
   }
   */
}
