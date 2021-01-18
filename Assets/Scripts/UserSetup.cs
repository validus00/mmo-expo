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
            Destroy(transform.GetComponent<MovementController>());
            Destroy(FPSCamera);
        } else {
            Destroy(userNameText);
        }

        SetUserUI();
    }

    void SetUserUI() {
        if (userNameText != null) {
            userNameText.text = photonView.Owner.NickName;
        }
    }
}
