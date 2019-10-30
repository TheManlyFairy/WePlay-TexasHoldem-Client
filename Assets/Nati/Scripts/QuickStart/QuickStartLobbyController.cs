using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject quickJoinButton;
    [SerializeField]
    GameObject quickCancelButton;
    [SerializeField]
    InputField roomNumber;
    [SerializeField]
    InputField playerNameInput;


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
        PhotonNetwork.JoinRoom("Room " + roomNumber.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Faild to join room: " + "Room " + roomNumber.text);
        quickJoinButton.SetActive(true);
    }

    public void QuickCancel()
    {
        quickCancelButton.SetActive(false);
        quickJoinButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

}
