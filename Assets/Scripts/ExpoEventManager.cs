using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ExpoEventManager : MonoBehaviourPunCallbacks {

    private const int __MAX_AVATAR_COUNT = 2;
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
        isNameInputTouched = false;
        DisplayAvatarPanel();
    }

    public void LeaveEvent() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        PhotonNetwork.Disconnect();
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

        if (canProceed) {
            SetPlayerName();

            int avatarPick = UserInfo.info.mySelectedAvatar;
            GameObject user = listOfAvatars[avatarPick];

            if (user != null) {
                Destroy(GameObject.Find(GameConstants.k_Camera));
                int randomPoint = Random.Range(-20, 20);
                PhotonNetwork.Instantiate(user.name, new Vector3(randomPoint, 0, randomPoint), Quaternion.identity);
                AvatarPanel.SetActive(false);
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
        return UserInfo.info.mySelectedAvatar < __MAX_AVATAR_COUNT;
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

    public void CheckNameAvailability() {
        Debug.Log("initial name is : " + initialName);
        bool isValidName = __IsNameAvailable() && isNameInputTouched;
        __ToggleNameText(isValidName);
    }

    public void SetPlayerName() {
        PhotonNetwork.NickName = initialName;
    }
}
