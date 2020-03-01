using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject loginOptionsButtonGroup;
    [SerializeField]
    GameObject quickJoinButtonsGroup;
    [SerializeField]
    GameObject quickJoinButton;
    [SerializeField]
    GameObject quickCancelButton;
    [SerializeField]
    InputField playerNameInput;
    [SerializeField]
    InputField roomNumberInput;
    [SerializeField]
    Text debugText;

    public static QuickStartLobbyController instance;
    public string DebugText { set { debugText.text = value; } }

    private void Awake()
    {
        if(instance == null)
        instance = this;

        DebugText = "";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickJoinButton.SetActive(true);

        if (PlayerPrefs.HasKey("NickName"))
        {
            if (PlayerPrefs.GetString("NickName") == "")
            {
                PhotonNetwork.NickName = "Player " + Random.Range(1, 1000);
            }
            else
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            }
        }
        else
        {
            PhotonNetwork.NickName = "Player " + Random.Range(1, 1000);
        }

        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdate()
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString("NickName", playerNameInput.text);
    }

    public void QuickJoinRoom()
    {
        quickJoinButton.SetActive(false);
        quickCancelButton.SetActive(true);
        PhotonNetwork.JoinRoom("Room " + roomNumberInput.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugText = "Faild to join room: " + "Room " + roomNumberInput.text;
        quickJoinButton.SetActive(true);
    }

    public void QuickCancel()
    {
        quickCancelButton.SetActive(false);
        quickJoinButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void SetupGoogleHUD()
    {
        loginOptionsButtonGroup.SetActive(false);
        roomNumberInput.gameObject.SetActive(true);
        quickJoinButtonsGroup.SetActive(true);
    }
}
