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
    private bool _joinExisting;
    private string _passcodeInput;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void ConnectToPhotonServer()
    {
        SetInitialName();
        EnterEventPanel.SetActive(false);
        ConnectionStatusPanel.SetActive(true);
        InvalidPasscodeText.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewRoom()
    {
        _joinExisting = false;
        ConnectToPhotonServer();
    }

    public void JoinCreatedRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            _joinExisting = true;
            ConnectToPhotonServer();
        }
    }

    private void SetInitialName()
    {
        PhotonNetwork.NickName = string.Format("User{0}", GenerateRandomDigits());
    }

    public void JoinExistingRoom()
    {
        if (string.IsNullOrEmpty(_passcodeInput))
        {
            Debug.Log("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(_passcodeInput);
    }

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

    public void DisplayMainMenu()
    {
        ResetPasscodePanel();
        EnterEventPanel.SetActive(true);
        PasscodeInputField.Select();
        PasscodeInputField.text = string.Empty;
        PhotonNetwork.Disconnect();
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

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully!");
        LoadEvent();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "!");
    }

    #endregion
}
