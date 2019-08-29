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
    InputField roomName;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickJoinButton.SetActive(true);
    }

    public void QuickJoinRoom()
    {
        quickJoinButton.SetActive(false);
        quickCancelButton.SetActive(true);
        PhotonNetwork.JoinRoom("Room " + roomName.text);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Faild to join room: " + "Room " + roomName.text);
        quickJoinButton.SetActive(true);
    }

    public void QuickCancel()
    {
        quickCancelButton.SetActive(false);
        quickJoinButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

}
