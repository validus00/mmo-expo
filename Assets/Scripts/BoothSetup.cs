using UnityEngine;

/*
 * BoothSetup class is for implementing business logic surrounding entering and leaving booths
 */
public class BoothSetup : MonoBehaviour {
    private BoothManager __boothManager;
    private ChatManager __chatManager;
    private Camera __camera;
    private bool isUserInBooth;
    public string projectName;
    public string teamName;
    public string projectDescription;
    public string urlText;
    
    void Start() {
        __boothManager = GameObject.Find("BoothManager").GetComponent<BoothManager>();
        __chatManager = GameObject.Find(GameConstants.k_ExpoEventManager).GetComponent<ChatManager>();
    }

    // This is called when the user first enters a booth area
    private void OnTriggerEnter(Collider other) {
        if (__camera == null) {
            __camera = GameObject.Find(GameConstants.k_Camera).GetComponent<Camera>();
            GetComponentInChildren<Canvas>().worldCamera = __camera;
        }

        if (other.name == GameConstants.k_MyUser && !string.IsNullOrWhiteSpace(projectName)) {
            __chatManager.UpdateChannel(projectName, ChatManager.ChannelType.boothChannel);
            __chatManager.EnterChannel(projectName);
        }

        isUserInBooth = true;
    }

    // This is called when the user leaves a booth area
    private void OnTriggerExit(Collider other) {
        if (other.name == GameConstants.k_MyUser) {
            __chatManager.UpdateChannel(string.Empty, ChatManager.ChannelType.boothChannel);
            __chatManager.LeaveChannel(projectName);
        }

        isUserInBooth = false;
    }

    public void OpenBoothInfoPanel() {
        __boothManager.OpenBoothInfoPanel(projectName, teamName, projectDescription, urlText);
    }
}
