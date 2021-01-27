using Photon.Pun;
using TMPro;
using UnityEngine;

/*
 * BoothSetup class is for implementing business logic surrounding entering and leaving booths
 */
public class BoothSetup : MonoBehaviourPunCallbacks {
    public TextMeshProUGUI boothText;
    private PanelManager __panelManager;
    private ChatManager __chatManager;
    private PhotonView __photonView;
    private Camera __camera;
    private bool __isUserInBooth = false;
    private string __projectName;
    private string __teamName;
    private string __projectDescription;
    private string __urlText;
    private bool __isOwner = false;
    
    void Start() {
        __photonView = GetComponent<PhotonView>();
        __panelManager = GameObject.Find(GameConstants.k_PanelManager).GetComponent<PanelManager>();
        __chatManager = GameObject.Find(GameConstants.k_ExpoEventManager).GetComponent<ChatManager>();
    }

    // This is called when the user first enters a booth area
    private void OnTriggerEnter(Collider other) {
        if (__camera == null) {
            __camera = GameObject.Find(GameConstants.k_Camera).GetComponent<Camera>();
            GetComponentInChildren<Canvas>().worldCamera = __camera;
        }

        if (other.name == GameConstants.k_MyUser) {
            __isUserInBooth = true;
            if (!string.IsNullOrWhiteSpace(__projectName)) {
                __JoinChannel();
            }
        }
    }

    // This is called when the user leaves a booth area
    private void OnTriggerExit(Collider other) {
        if (other.name == GameConstants.k_MyUser) {
            __LeaveChannel();
            __isUserInBooth = false;
        }
    }

    private void __JoinChannel() {
        __chatManager.UpdateChannel(__projectName, ChatManager.ChannelType.boothChannel);
        __chatManager.EnterChannel(__projectName);
    }

    private void __LeaveChannel() {
        __chatManager.UpdateChannel(string.Empty, ChatManager.ChannelType.boothChannel);
        __chatManager.LeaveChannel(__projectName);
    }

    public void OpenBoothPanel() {
        if (__isUserInBooth) {
            if (string.IsNullOrEmpty(__projectName)) {
                __panelManager.OpenBoothFormPanel(this);
            } else {
                __panelManager.OpenBoothInfoPanel(this, __isOwner, __projectName, __teamName, __projectDescription, __urlText);
            }
        }
    }

    public void SetUpBooth(string projectName, string teamName, string projectDescription, string urlText) {
        __isOwner = true;
        __photonView.RPC("SyncBooth", RpcTarget.AllBuffered, projectName, teamName, projectDescription, urlText);
    }

    [PunRPC]
    void SyncBooth(string projectName, string teamName, string projectDescription, string urlText) {
        boothText.text = projectName;
        AssignBoothValues(projectName, teamName, projectDescription, urlText);
        if (__isUserInBooth) {
            __JoinChannel();
        }
    }

    private void AssignBoothValues(string projectName, string teamName, string projectDescription, string urlText) {
        __projectName = projectName;
        __teamName = teamName;
        __projectDescription = projectDescription;
        __urlText = urlText;
    }

    public void ResetBooth() {
        __isOwner = false;
        __photonView.RPC("ClearBooth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ClearBooth() {
        if (__isUserInBooth) {
            __LeaveChannel();
        }
        boothText.text = string.Empty;
        AssignBoothValues(string.Empty, string.Empty, string.Empty, string.Empty);
    }
}
