using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterEventPanel;
    public GameObject ConnectionStatusPanel;
    public GameObject AvatarPanel;
    public GameObject EnterPasscodePanel;
    public GameObject InvalidPasscodeText;
    public bool createNew;
    public bool joinExisting;
    public string passcodeInput;

    #region Unity Methods
    void Start() {
        EnterEventPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        AvatarPanel.SetActive(false);
        EnterPasscodePanel.SetActive(false);
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
            EnterPasscodePanel.SetActive(false);
        }
    }

    public void JoinRandomRoom() {
        createNew = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public void DisplayPasscodePanel() {
        joinExisting = true;
        EnterPasscodePanel.SetActive(true);
        InvalidPasscodeText.SetActive(false);
    }

    public void JoinExistingRoom()
    {
        if (string.IsNullOrEmpty(passcodeInput)) {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(passcodeInput);
    

    }

    public void SetPasscode(string passcode) {
        if (string.IsNullOrEmpty(passcode)) {
            Debug.Log("Passcode is empty");
            return;
        }

        Debug.Log("passcode: " + passcode);
        passcodeInput = passcode;
    }

    #endregion


    #region Photon Callbacks
    public override void OnConnectedToMaster() {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is creating new event: " + createNew);
        Debug.Log("Is joining existing event: " + joinExisting);
        if (createNew) {
            AvatarPanel.SetActive(true);
            ConnectionStatusPanel.SetActive(false);
            JoinRandomRoom();
        } else if (joinExisting) {
            ConnectionStatusPanel.SetActive(false);
            EnterPasscodePanel.SetActive(true);
            InvalidPasscodeText.SetActive(false);
        }
        
    }

    // This method is called when we have internet connection (before OnConnectedToMaster)
    public override void OnConnected() {
        Debug.Log("Internet!!");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("No room exists!");
        if (!joinExisting) {
            CreateAndJoinRoom();
        } else {
            Debug.Log("Cannot join room");
        }
        
    }

    // Called when PhotonNetwork.JoinRoom fails
    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("joined room failed");
        InvalidPasscodeText.SetActive(true);
    }


    public override void OnJoinedRoom() {
        Debug.Log("Joined room successfully!");
        if (joinExisting) {
            EnterPasscodePanel.SetActive(false);
        }
        AvatarPanel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "!");
    }

    #endregion

    #region Private Methods

    void CreateAndJoinRoom() {

        string randomRoomName = "" + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        Debug.Log("This is the newly created room name: " + randomRoomName);

    }


    #endregion
}
