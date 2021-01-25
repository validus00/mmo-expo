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


    // Start is called before the first frame update
    void Start() {
        if (!photonView.IsMine) {
            Destroy(GetComponent<MovementController>());
            Destroy(FPSCamera);
        } else {
            // Differentiate user's own User clone from other clones with a different name
            transform.name = "MyUser";
            Destroy(userNameText);
        }

        SetUserUI();
    }

    void SetUserUI() {
        if (userNameText != null && photonView.Owner != null) {
            userNameText.text = photonView.Owner.NickName;
        }
    }
}
