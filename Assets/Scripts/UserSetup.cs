using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UserSetup : MonoBehaviourPunCallbacks {
    [SerializeField]
    GameObject FPSCamera;

    [SerializeField]
    TextMeshProUGUI userNameText;

    private CharacterController __characterController;

    // Start is called before the first frame update
    void Start() {
        __characterController = GetComponent<CharacterController>();

        if (!photonView.IsMine) {
            Destroy(transform.GetComponent<MovementController>());
            Destroy(FPSCamera);
        } else {
            // Differentiate user's own User clone from other clones with a different name
            transform.name = "MyUser";
            Destroy(userNameText);
        }

        SetUserUI();
    }

    void SetUserUI() {
        if (userNameText != null) {
            userNameText.text = photonView.Owner.NickName;
        }
    }


    [PunRPC]
    public void CharacterControllerToggle(bool value) {
        if (!GetComponent<PhotonView>().IsMine) {
            __characterController.enabled = value;
        }
    }

}
