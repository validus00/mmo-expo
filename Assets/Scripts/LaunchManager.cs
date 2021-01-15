using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterEventPanel;
    public GameObject ConnectionStatusPanel;
    public GameObject AvatarPanel;

    #region Unity Methods
    void Start() {
        EnterEventPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        AvatarPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }

    #endregion


    #region Public Methods
    public void ConnectToPhotonServer() {
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings();
            ConnectionStatusPanel.SetActive(true);
            EnterEventPanel.SetActive(false);
            AvatarPanel.SetActive(false);
        }
    }

    public void JoinRandomRoom() {
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion


    #region Photon Callbacks
    public override void OnConnectedToMaster() {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        AvatarPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        JoinRandomRoom();
    }

    // This method is called when we have internet connection (before OnConnectedToMaster)
    public override void OnConnected() {
        Debug.Log("Internet!!");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("No room exists!");
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom() {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    #endregion

    #region Private Methods

    void CreateAndJoinRoom() {
        string randomRoomName = "" + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
    #endregion
}
