using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LaunchManager : MonoBehaviourPunCallbacks {
    public GameObject enterEventPanel;
    public GameObject connectionStatusPanel;
    public GameObject enterPasscodePanel;
    public GameObject invalidPasscodeText;
    public InputField passcodeInputField;
    private bool __joinExisting;
    private string __passcodeInput;

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void __ConnectToPhotonServer() {
        __SetInitialName();
        enterEventPanel.SetActive(false);
        connectionStatusPanel.SetActive(true);
        invalidPasscodeText.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewRoom() {
        __joinExisting = false;
        __ConnectToPhotonServer();
    }

    public void JoinCreatedRoom() {
        if (!PhotonNetwork.IsConnected) {
            __joinExisting = true;
            __ConnectToPhotonServer();
        }
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

    private void __CreateAndJoinRoom() {
        string randomRoomName = "" + __GenerateRandomDigits();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;
        roomOptions.CleanupCacheOnLeave = false;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        Debug.Log("This is the newly created room name: " + randomRoomName);
    }

    private int __GenerateRandomDigits() {
        return Random.Range(1000, 9999);
    }

    public void DisplayMainMenu() {
        __ResetPasscodePanel();
        enterEventPanel.SetActive(true);
        passcodeInputField.Select();
        passcodeInputField.text = string.Empty;
        PhotonNetwork.Disconnect();
    }

    private void __ResetPasscodePanel() {
        enterPasscodePanel.SetActive(false);
        invalidPasscodeText.SetActive(false);
        __passcodeInput = string.Empty;
        __joinExisting = false;
    }

    private void __DisplayPasscodePanel() {
        enterPasscodePanel.SetActive(true);
        invalidPasscodeText.SetActive(false);
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster() {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is joining existing event: " + __joinExisting);

        if (__joinExisting) {
            __DisplayPasscodePanel();
        } else {
            __CreateAndJoinRoom();
        }
        
    }

    // This method is called when we have internet connection (before OnConnectedToMaster)
    public override void OnConnected() {
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
}
