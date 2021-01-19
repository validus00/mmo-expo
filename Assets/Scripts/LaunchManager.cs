using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks {
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

    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
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

    public void CreateNewRoom() {
        if (!string.IsNullOrWhiteSpace(PhotonNetwork.NickName)) {
            createNew = true;
            ConnectToPhotonServer();
        }
    }

    public void JoinCreatedRoom() {
        if (!string.IsNullOrWhiteSpace(PhotonNetwork.NickName)) {
            joinExisting = true;
            ConnectToPhotonServer();
        }
    }

    public void DisplayPasscodePanel() {
        AvatarPanel.SetActive(true);
        //EnterPasscodePanel.SetActive(true);
        //InvalidPasscodeText.SetActive(false);
    }

    public void JoinExistingRoom() {
        if (string.IsNullOrEmpty(passcodeInput)) {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(passcodeInput);
        EnterPasscodePanel.SetActive(false);
        AvatarPanel.SetActive(true);
    }

    public void SetPasscode(string passcode) {
        if (string.IsNullOrEmpty(passcode)) {
            Debug.Log("Passcode is empty");
            return;
        }

        Debug.Log("passcode: " + passcode);
        passcodeInput = passcode;
    }


    public void nextPanel() {
        if (joinExisting) {
            EnterPasscodePanel.SetActive(true);
            AvatarPanel.SetActive(false);
        } else {
            LoadEvent();
        }
    }
    public void LoadEvent() {
        //if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("MainHall");
        //}
        
    }

    #endregion


    #region Photon Callbacks
    public override void OnConnectedToMaster() {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is creating new event: " + createNew);
        Debug.Log("Is joining existing event: " + joinExisting);
        if (createNew) {
            CreateAndJoinRoom();
        } else if (joinExisting) {
            DisplayPasscodePanel();
        }
        
    }

    // This method is called when we have internet connection (before OnConnectedToMaster)
    public override void OnConnected() {
        Debug.Log("Internet!!");
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
        ConnectionStatusPanel.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "!");
    }

    #endregion

    #region Private Methods

    void CreateAndJoinRoom() {

        string randomRoomName = "" + Random.Range(1000, 9999);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        Debug.Log("This is the newly created room name: " + randomRoomName);

    }


    #endregion
}
