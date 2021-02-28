using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/*
 * LaunchManager class is for implementing business logic surrounding launching the event for new users
 */
public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject EnterEventPanel;
    public GameObject ConnectionStatusPanel;
    public GameObject EnterPasscodePanel;
    public GameObject InvalidPasscodeText;
    public InputField PasscodeInputField;
    private bool _joinExisting;
    private string _passcodeInput;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void ConnectToPhotonServer()
    {
        SetInitialName();
        PhotonNetwork.ConnectUsingSettings();
    }

    private void CloseEnterEventPanel()
    {
        EnterEventPanel.SetActive(false);
        ConnectionStatusPanel.SetActive(true);
    }

    // For when user clicks on creating new room
    public void CreateNewRoom()
    {
        CloseEnterEventPanel();
        if (PhotonNetwork.IsConnected)
        {
            CreateAndJoinRoom();
        }
        else
        {
            _joinExisting = false;
            ConnectToPhotonServer();
        }
    }

    // For when user clicks on join creatd room 
    public void JoinCreatedRoom()
    {
        CloseEnterEventPanel();
        if (PhotonNetwork.IsConnected)
        {
            DisplayPasscodePanel();
        }
        else
        {
            _joinExisting = true;
            ConnectToPhotonServer();
        }
    }

    private void SetInitialName()
    {
        PhotonNetwork.NickName = string.Format("User{0}", GenerateRandomDigits());
    }

    // For handling passcode and room joining logic
    public void JoinExistingRoom()
    {
        if (string.IsNullOrEmpty(_passcodeInput))
        {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(_passcodeInput);
    }

    // For handling passcode-entering logic
    public void SetPasscode(string passcode)
    {
        if (string.IsNullOrEmpty(passcode))
        {
            Debug.Log("Passcode is empty");
            return;
        }

        Debug.Log("passcode: " + passcode);
        _passcodeInput = passcode;
    }

    private void LoadEvent()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainEventScene");
        }
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "" + GenerateRandomDigits();
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
        CreateAndJoinRoom();
    }

    private int GenerateRandomDigits()
    {
        return Random.Range(1000, 9999);
    }

    // For displaying the main menu
    public void DisplayMainMenu()
    {
        ResetPasscodePanel();
        EnterEventPanel.SetActive(true);
        PasscodeInputField.Select();
        PasscodeInputField.text = string.Empty;
    }

    private void ResetPasscodePanel()
    {
        EnterPasscodePanel.SetActive(false);
        InvalidPasscodeText.SetActive(false);
        _passcodeInput = string.Empty;
        _joinExisting = false;
    }

    private void DisplayPasscodePanel()
    {
        EnterPasscodePanel.SetActive(true);
        InvalidPasscodeText.SetActive(false);
    }

    #region Photon Callbacks
    // Called when user connects to lobby
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is joining existing event: " + _joinExisting);

        if (_joinExisting)
        {
            DisplayPasscodePanel();
        }
        else
        {
            CreateAndJoinRoom();
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

    // Called when user joins a room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully!");
        LoadEvent();
    }

    // Called when user enters a room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "!");
    }

    #endregion
}
