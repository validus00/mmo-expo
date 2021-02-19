using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UserSetup : MonoBehaviourPunCallbacks
{
    public GameObject FPSCamera;
    public TextMeshProUGUI userNameText;
    public GameObject user;
    private CharacterController __characterController;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<MovementController>());
            Destroy(FPSCamera);
        }
        else
        {
            // Differentiate user's own User clone from other clones with a different name
            transform.name = GameConstants.k_MyUser;
            Destroy(userNameText);
        }

        SetUserUI();
    }

    private void Awake()
    {
        __characterController = GetComponent<CharacterController>();
    }

    void SetUserUI()
    {
        if (userNameText != null && photonView.Owner != null)
        {
            userNameText.text = PhotonChatHandler.RemoveRoomName(photonView.Owner.NickName);
        }
    }


    [PunRPC]
    public void CharacterControllerToggle(bool value)
    {
        if (__characterController == null)
        {
            __characterController = GetComponent<CharacterController>();
        }
        if (!GetComponent<PhotonView>().IsMine)
        {
            __characterController.enabled = value;
        }
    }

    [PunRPC]
    public void RemoveUser()
    {
        Destroy(user);
    }
}
