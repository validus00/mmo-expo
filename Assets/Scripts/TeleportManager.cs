using Photon.Pun;
using TMPro;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public Transform TeleportTarget;
    public TextMeshProUGUI DestinationText;
    private ChatManager _chatManager;
#pragma warning disable 649
    [SerializeField]
    private string _currentChannelName;
#pragma warning restore 649

    void Start()
    {
        _chatManager = GameObject.Find(GameConstants.k_ExpoEventManager).GetComponent<ChatManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (other.gameObject.name.Equals("MyUser"))
        {
            // Turn off the character controller so that the user can be teleported
            controller.enabled = false;
            other.GetComponent<PhotonView>().RPC("CharacterControllerToggle", RpcTarget.AllBuffered, controller.enabled);
            other.transform.position = TeleportTarget.transform.position;
            ChangeChannel();
        }
    }

    private void ChangeChannel()
    {
        string newChannelName = DestinationText.text;
        _chatManager.UpdateChannel(newChannelName, ChatManager.ChannelType.hallChannel);
        _chatManager.LeaveChannel(_currentChannelName);
        _chatManager.EnterChannel(newChannelName);
    }
}
