using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ExpoEventManager : MonoBehaviourPunCallbacks {

    private const int __MAX_AVATAR_COUNT = 2;
    private List<string> names;
    public GameObject AvatarPanel;
    public GameObject UnselectedAvatarText;
    public GameObject DuplicateNameText;
    public GameObject NameIsAvailableText;
    public GameObject EnterNameText;
    public GameObject[] listOfAvatars;
    public static string initialName;
    public static bool isNameInputTouched;

    // Start is called before the first frame update
    void Start() {
        initialName = PhotonNetwork.NickName;
        names = new List<string>();
        isNameInputTouched = false;
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel("EventLauncherScene");
    }

    public void DisplayAvatarPanel() {
        AvatarPanel.SetActive(true);
        UnselectedAvatarText.SetActive(false);
        NameIsAvailableText.SetActive(false);
        DuplicateNameText.SetActive(false);
        EnterNameText.SetActive(false);
    }

    // Used to determine which panel gets loaded after the avatar panel
    // Joining existing will load passcode panel and creating new will load level
    public void NextPanel() {
        bool isApprovedAvatar = __IsAvatarSelected();
        bool isApprovedName = __IsNameAvailable() && isNameInputTouched;
        bool canProceed = isApprovedAvatar && isApprovedName;

        // ??
        names.Add(PhotonNetwork.NickName);

        if (canProceed) {
            SetPlayerName();

            int avatarPick = PlayerPrefs.GetInt(GameConstants.k_MyAvatar);
            GameObject user = listOfAvatars[avatarPick];

            if (user != null) {
                int randomPoint = Random.Range(-20, 20);
                PhotonNetwork.Instantiate(user.name, new Vector3(randomPoint, 0, randomPoint), Quaternion.identity);
            }
        } else {
            __ToggleAvatarText(isApprovedAvatar);
            __ToggleNameText(isApprovedName);

        }
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

    private int __GenerateRandomDigits() {
        return Random.Range(1000, 9999);
    }

    public void CheckNameAvailability() {
        Debug.Log("initial name is : " + initialName);
        bool isValidName = __IsNameAvailable() && isNameInputTouched;
        __ToggleNameText(isValidName);
    }

    public void SetPlayerName() {
        PhotonNetwork.NickName = initialName;
    }
}
