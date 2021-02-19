using Photon.Pun;
using TMPro;
using UnityEngine;

public class UserSetup : MonoBehaviourPunCallbacks
{
    public GameObject FpsCamera;
    public TextMeshProUGUI UserNameText;
    public GameObject User;
    private CharacterController _characterController;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<MovementController>());
            Destroy(FpsCamera);
        }
        else
        {
            // Differentiate user's own User clone from other clones with a different name
            transform.name = GameConstants.k_MyUser;
            Destroy(UserNameText);
        }

        SetUserUI();
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void SetUserUI()
    {
        if (UserNameText != null && photonView.Owner != null)
        {
            UserNameText.text = PhotonChatHandler.RemoveRoomName(photonView.Owner.NickName);
        }
    }


    [PunRPC]
    public void CharacterControllerToggle(bool value)
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }
        if (!GetComponent<PhotonView>().IsMine)
        {
            _characterController.enabled = value;
        }
    }

    [PunRPC]
    public void RemoveUser()
    {
        Destroy(User);
    }
}
