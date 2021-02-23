using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ExpoEventManager : MonoBehaviourPunCallbacks
{

    private const int K_maxAvatarCount = 2;
    public GameObject AvatarPanel;
    public GameObject UnselectedAvatarText;
    public GameObject DuplicateNameText;
    public GameObject NameIsAvailableText;
    public GameObject EnterNameText;
    public GameObject[] ListOfAvatars;
    public static string InitialName;
    public static bool IsNameInputTouched;
    public static bool IsNameUpdated;
    private int _mySelectedAvatar;
    private string _roomName;

    void Awake()
    {
        _roomName = PhotonNetwork.CurrentRoom.Name;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialName = PhotonNetwork.NickName;
        _mySelectedAvatar = 2;
        IsNameInputTouched = false;
        IsNameUpdated = false;
        DisplayAvatarPanel();
    }
    public string PassCode
    {
        get { return _roomName; }
    }

    public void LeaveEvent()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        ResetEvent();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (GameObject user in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = user.GetPhotonView();
            if (photonView.Owner == null || photonView.Owner.NickName == otherPlayer.NickName)
            {
                photonView.RPC("RemoveUser", RpcTarget.AllBuffered);
                break;
            }
        }
    }

    public void DisplayAvatarPanel()
    {
        AvatarPanel.SetActive(true);
        UnselectedAvatarText.SetActive(false);
        NameIsAvailableText.SetActive(false);
        DuplicateNameText.SetActive(false);
        EnterNameText.SetActive(false);
    }

    // Used to determine which panel gets loaded after the avatar panel
    // Joining existing will load passcode panel and creating new will load level
    public void NextPanel()
    {
        bool isApprovedAvatar = IsAvatarSelected();
        bool isApprovedName = IsNameAvailable() && IsNameInputTouched;
        bool canProceed = isApprovedAvatar && isApprovedName;

        if (canProceed)
        {
            SetPlayerName();

            GameObject user = ListOfAvatars[_mySelectedAvatar];

            if (user != null)
            {
                Destroy(GameObject.Find(GameConstants.K_Camera));
                int randomPoint = Random.Range(-20, 20);
                PhotonNetwork.Instantiate(user.name, new Vector3(randomPoint, 0, randomPoint), Quaternion.identity);
                AvatarPanel.SetActive(false);
            }
        }
        else
        {
            ToggleAvatarText(isApprovedAvatar);
            ToggleNameText(isApprovedName);

        }
    }

    private bool IsNameAvailable()
    {
        string name = InitialName + PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (name.Equals(player.NickName))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsAvatarSelected()
    {
        return _mySelectedAvatar < K_maxAvatarCount;
    }

    private void ToggleNameText(bool isValidName)
    {
        if (isValidName)
        {
            NameIsAvailableText.SetActive(true);
            DuplicateNameText.SetActive(false);
            EnterNameText.SetActive(false);
        }
        else
        {
            if (!IsNameInputTouched)
            {
                EnterNameText.SetActive(true);
                NameIsAvailableText.SetActive(false);
                DuplicateNameText.SetActive(false);
            }
            else
            {
                EnterNameText.SetActive(false);
                NameIsAvailableText.SetActive(false);
                DuplicateNameText.SetActive(true);
            }

        }
    }

    private void ToggleAvatarText(bool isValidAvatar)
    {
        if (!isValidAvatar)
        {
            UnselectedAvatarText.SetActive(true);
        }
        else
        {
            UnselectedAvatarText.SetActive(false);
        }
    }

    public void CheckNameAvailability()
    {
        Debug.Log("initial name is : " + InitialName);
        bool isValidName = IsNameAvailable() && IsNameInputTouched;
        ToggleNameText(isValidName);
    }

    public void SetPlayerName()
    {
        PhotonNetwork.NickName = InitialName + PhotonNetwork.CurrentRoom.Name;
        IsNameUpdated = true;
    }

    public void OnClickAvatarSelection(int avatar)
    {
        _mySelectedAvatar = avatar;
    }

    private void ResetEvent()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("EventLauncherScene");
    }
}
