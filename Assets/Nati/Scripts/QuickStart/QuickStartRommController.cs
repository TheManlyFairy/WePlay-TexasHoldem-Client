using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class QuickStartRommController : MonoBehaviourPunCallbacks
{

    [SerializeField]
    int multiplayerSceneIndex;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public override void OnJoinedRoom()
    {
        QuickStartLobbyController.instance.DebugText = "Joined Room " + PhotonNetwork.CurrentRoom.Name;
        StartGame();
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            QuickStartLobbyController.instance.DebugText = "Startiong Game";
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }
}
