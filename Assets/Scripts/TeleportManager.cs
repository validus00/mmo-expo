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
        _chatManager = GameObject.Find(GameConstants.K_ExpoEventManager).GetComponent<ChatManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("MyUser"))
        {
            Teleport(other.gameObject, TeleportTarget.transform.position);
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

    public static void Teleport(GameObject user, Vector3 destination)
    {
        // Turn off the character controller so that the user can be teleported
        CharacterController controller = user.GetComponent<CharacterController>();
        controller.enabled = false;
        PhotonView photonView = user.GetComponent<PhotonView>();
        if (photonView != null)
        {
            photonView.RPC("CharacterControllerToggle", RpcTarget.AllBuffered, controller.enabled);
        }
        user.transform.position = destination;
    }
}
