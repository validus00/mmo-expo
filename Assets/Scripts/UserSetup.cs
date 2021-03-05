using Photon.Pun;
using TMPro;
using UnityEngine;

/*
 * UserSetup class implements user specific features
 */
public class UserSetup : MonoBehaviour
{
    public GameObject FpsCamera;
    public TextMeshProUGUI UserNameText;
    public GameObject User;
    private CharacterController _characterController;
    private PhotonView _photonView;

    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine)
        {
            Destroy(GetComponent<MovementController>());
            Destroy(FpsCamera);
        }
        else
        {
            // Differentiate user's own User clone from other clones with a different name
            transform.name = GameConstants.K_MyUser;
            Destroy(UserNameText);
        }

        SetUserUI();
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void SetUserUI()
    {
        if (UserNameText != null && _photonView.Owner != null)
        {
            UserNameText.text = PhotonChatHandler.RemoveRoomName(_photonView.Owner.NickName);
        }
    }

    // RPC for enable or disabling character controllers on users while teleporting them
    [PunRPC]
    public void CharacterControllerToggle(bool value)
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }
        if (!_photonView.IsMine)
        {
            _characterController.enabled = value;
        }
    }

    // RPC for removing user clones who have left the event
    [PunRPC]
    public void RemoveUser()
    {
        Destroy(User);
    }
}
