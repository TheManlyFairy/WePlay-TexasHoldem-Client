using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
public class LoginManager : MonoBehaviour
{
    public QuickStartLobbyController lobbyController;
    public static PlayGamesPlatform platform;
    public Text debugText;

    // Start is called before the first frame update
    public void GoogleLogin()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;

        platform = PlayGamesPlatform.Activate();

        Social.Active.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Logged in successfully");
                //debugText.text = Social.Active.localUser.userName;
                Photon.Pun.PhotonNetwork.NickName = Social.Active.localUser.userName;
                lobbyController.SetupGoogleHUD();
            }
            else
            {
                Debug.Log("Failed to log in!!!");
                debugText.text = "Could not connect to Google Play Services";
            }
        });
    }
}
