using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterEventPanel;
    public GameObject ConnectionStatusPanel;
    public GameObject EnterPasscodePanel;
    public GameObject InvalidPasscodeText;
    public InputField PasscodeInputField;
    private bool __joinExisting;
    private string __passcodeInput;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void __ConnectToPhotonServer()
    {
        __SetInitialName();
        EnterEventPanel.SetActive(false);
        ConnectionStatusPanel.SetActive(true);
        InvalidPasscodeText.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewRoom()
    {
        __joinExisting = false;
        __ConnectToPhotonServer();
    }

    public void JoinCreatedRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            __joinExisting = true;
            __ConnectToPhotonServer();
        }
    }

    private void __SetInitialName()
    {
        PhotonNetwork.NickName = string.Format("User{0}", __GenerateRandomDigits());
    }

    public void JoinExistingRoom()
    {
        if (string.IsNullOrEmpty(__passcodeInput))
        {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(__passcodeInput);
    }

    public void SetPasscode(string passcode)
    {
        if (string.IsNullOrEmpty(passcode))
        {
            Debug.Log("Passcode is empty");
            return;
        }

        Debug.Log("passcode: " + passcode);
        __passcodeInput = passcode;
    }

    private void __LoadEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainEventScene");
        }
    }

    private void __CreateAndJoinRoom()
    {
        string randomRoomName = "" + __GenerateRandomDigits();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 20;
        roomOptions.CleanupCacheOnLeave = false;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        Debug.Log($"Creating room with name: {randomRoomName}");
    }

    // This callback is called if the randomly generated room name is identical to one that is already used by an
    // existing room
    public void OnCreateRoomFailed()
    {
        Debug.Log("Create room failed");
        __CreateAndJoinRoom();
    }

    private int __GenerateRandomDigits()
    {
        return Random.Range(1000, 9999);
    }

    public void DisplayMainMenu()
    {
        __ResetPasscodePanel();
        EnterEventPanel.SetActive(true);
        PasscodeInputField.Select();
        PasscodeInputField.text = string.Empty;
        PhotonNetwork.Disconnect();
    }

    private void __ResetPasscodePanel()
    {
        EnterPasscodePanel.SetActive(false);
        InvalidPasscodeText.SetActive(false);
        __passcodeInput = string.Empty;
        __joinExisting = false;
    }

    private void __DisplayPasscodePanel()
    {
        EnterPasscodePanel.SetActive(true);
        InvalidPasscodeText.SetActive(false);
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is joining existing event: " + __joinExisting);

        if (__joinExisting)
        {
            __DisplayPasscodePanel();
        }
        else
        {
            __CreateAndJoinRoom();
        }
    }

    // This method is called when we have internet connection (before OnConnectedToMaster)
    public override void OnConnected()
    {
        Debug.Log("Internet!!");
    }

    // Called when PhotonNetwork.JoinRoom fails
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("joined room failed");
        InvalidPasscodeText.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully!");
        __LoadEvent();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "!");
    }

    #endregion
}
