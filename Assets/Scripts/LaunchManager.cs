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
    public GameObject UnselectedAvatarText;
    public GameObject DuplicateNameText;
    public GameObject NameIsAvailableText;
    public GameObject EnterNameText;
    private bool __createNew;
    private bool __joinExisting;
    private string __passcodeInput;
    public static string initialName;
    public static bool isNameInputTouched;
    private int __MAX_AVATAR_COUNT = 2;
    private List<string> names;

    #region Unity Methods
    void Start() {
        initialName = "User" + __GenerateRandomDigits();
        names = new List<string>();
        isNameInputTouched = false;
        EnterEventPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        AvatarPanel.SetActive(false);
        EnterPasscodePanel.SetActive(false);
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
        if (!string.IsNullOrWhiteSpace(initialName)) {
            __createNew = true;
            ConnectToPhotonServer();
        }
    }

    public void JoinCreatedRoom() {
        if (!string.IsNullOrWhiteSpace(initialName)) {
            __joinExisting = true;
            ConnectToPhotonServer();
        }
    }

    public void DisplayAvatarPanel() {
        AvatarPanel.SetActive(true);
        UnselectedAvatarText.SetActive(false);
        NameIsAvailableText.SetActive(false);
        DuplicateNameText.SetActive(false);
        EnterNameText.SetActive(false);
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

    // Used to determine which panel gets loaded after the avatar panel
    // Joining existing will load passcode panel and creating new will load level
    public void NextPanel() {
        bool isApprovedAvatar = __IsAvatarSelected();
        bool isApprovedName = __IsNameAvailable() && isNameInputTouched;
        bool canProceed = isApprovedAvatar && isApprovedName;

        if (canProceed) {
            SetPlayerName();
            if (__joinExisting) {
                EnterPasscodePanel.SetActive(true);
                InvalidPasscodeText.SetActive(false);
                AvatarPanel.SetActive(false);
            } else {
                LoadEvent();
            }
        } else {
            __ToggleAvatarText(isApprovedAvatar);
            __ToggleNameText(isApprovedName);

        }
    }

    public void LoadEvent() {
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("ShowcaseHalls");
        }
    }


    public void CheckNameAvailability() {
        Debug.Log("initial name is : " + initialName);
        bool isValidName = __IsNameAvailable() && isNameInputTouched;
        __ToggleNameText(isValidName);
    }

    public void SetPlayerName() {
        PhotonNetwork.NickName = initialName;
    }

    #endregion


    #region Photon Callbacks
    public override void OnConnectedToMaster() {
        Debug.Log("Connected! " + PhotonNetwork.NickName);
        Debug.Log("Is creating new event: " + __createNew);
        Debug.Log("Is joining existing event: " + __joinExisting);
        if (__createNew) {
            __CreateAndJoinRoom();
        } else if (__joinExisting) {
            DisplayAvatarPanel();
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
        names.Add(PhotonNetwork.NickName);
        DisplayAvatarPanel();
        ConnectionStatusPanel.SetActive(false);
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

    private bool __IsNameAvailable() {
        foreach (Player player in PhotonNetwork.PlayerList) { 
            if (initialName.Equals(player.NickName)) {
                return false;
            }
        }
        return true;
    }

    private bool __IsAvatarSelected() {
        return PlayerPrefs.GetInt("myAvatar") < __MAX_AVATAR_COUNT;
    }

    private void __ToggleNameText(bool isValidName) {
        if (isValidName) {
            NameIsAvailableText.SetActive(true);
            DuplicateNameText.SetActive(false);
            EnterNameText.SetActive(false);
        } else {
            if (!isNameInputTouched) {
                EnterNameText.SetActive(true);
                NameIsAvailableText.SetActive(false);
                DuplicateNameText.SetActive(false);
            } else {
                EnterNameText.SetActive(false);
                NameIsAvailableText.SetActive(false);
                DuplicateNameText.SetActive(true);
            }

        }
    }

    private void __ToggleAvatarText(bool isValidAvatar) {
        if (!isValidAvatar) {
            UnselectedAvatarText.SetActive(true);
        } else {
            UnselectedAvatarText.SetActive(false);
        }
    }


    #endregion
}
