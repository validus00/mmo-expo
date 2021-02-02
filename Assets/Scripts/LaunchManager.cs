using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks {
    public GameObject enterEventPanel;
    public GameObject connectionStatusPanel;
    public GameObject enterPasscodePanel;
    public GameObject invalidPasscodeText;
    private bool __joinExisting;
    private string __passcodeInput;

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Public Methods

    private void __ConnectToPhotonServer() {
        __SetInitialName();
        enterEventPanel.SetActive(false);

        if (PhotonNetwork.IsConnected) {
            if (__joinExisting) {
                enterPasscodePanel.SetActive(true);
                invalidPasscodeText.SetActive(false);
            } else {
                __CreateAndJoinRoom();
            }
        } else {
            connectionStatusPanel.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CreateNewRoom() {
        __joinExisting = false;
        __ConnectToPhotonServer();
    }

    public void JoinCreatedRoom() {
        __joinExisting = true;
        __ConnectToPhotonServer();
    }

    private void __SetInitialName() {
        PhotonNetwork.NickName = string.Format("User{0}", __GenerateRandomDigits());
    }

    public void JoinExistingRoom() {
        if (string.IsNullOrEmpty(__passcodeInput)) {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(__passcodeInput);
    }

    public void SetPasscode(string passcode) {
        if (string.IsNullOrEmpty(passcode)) {
            Debug.Log("Passcode is empty");
            return;
        }

        Debug.Log("passcode: " + passcode);
        __passcodeInput = passcode;
    }

    private void __LoadEvent() {
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("MainEventScene");
        }
    }

    #endregion


    #region Photon Callbacks

    public override void OnConnectedToMaster() {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is joining existing event: " + __joinExisting);

        if (__joinExisting) {
            enterPasscodePanel.SetActive(true);
            invalidPasscodeText.SetActive(false);
        } else {
            __CreateAndJoinRoom();
        }
        
    }

    // This method is called when we have internet connection (before OnConnectedToMaster)
    public override void OnConnected() {
        connectionStatusPanel.SetActive(false);
        Debug.Log("Internet!!");
    }

    // Called when PhotonNetwork.JoinRoom fails
    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("joined room failed");
        invalidPasscodeText.SetActive(true);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room successfully!");
        __LoadEvent();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "!");
    }

    #endregion


    #region Private Methods

    private void __CreateAndJoinRoom() {
        string randomRoomName = "" + __GenerateRandomDigits();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        Debug.Log("This is the newly created room name: " + randomRoomName);
    }

    private int __GenerateRandomDigits() {
        return Random.Range(1000, 9999);
    }

    #endregion
}
