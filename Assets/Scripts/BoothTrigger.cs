using UnityEngine;

/*
 * BoothTrigger class is for implementing business logic surrounding entering and leaving booths
 */
public class BoothTrigger : MonoBehaviour {
    public string projectName;

    // This is called when the user first enters a booth area
    private void OnTriggerEnter(Collider other) {
        if (other.name == GameConstants.k_MyUser && !string.IsNullOrWhiteSpace(projectName)) {
            ChatManager chatManager = GameObject.Find(GameConstants.k_ExpoEventManager).GetComponent<ChatManager>();
            chatManager.UpdateChannel(projectName, ChatManager.ChannelType.boothChannel);
            chatManager.EnterChannel(projectName);
        }
    }

    // This is called when the user leaves a booth area
    private void OnTriggerExit(Collider other) {
        if (other.name == GameConstants.k_MyUser) {
            ChatManager chatManager = GameObject.Find(GameConstants.k_ExpoEventManager).GetComponent<ChatManager>();
            chatManager.UpdateChannel(string.Empty, ChatManager.ChannelType.boothChannel);
            chatManager.LeaveChannel(projectName);
        }
    }
}
