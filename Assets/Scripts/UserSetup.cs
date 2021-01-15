using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UserSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject FPSCamera;

    [SerializeField]
    TextMeshProUGUI userNameText;


    // Start is called before the first frame update
    void Start() {
        if (photonView.IsMine) {
            transform.GetComponent<MovementController>().enabled = true;
            FPSCamera.GetComponent<Camera>().enabled = true;

        } else {
            transform.GetComponent<MovementController>().enabled = false;
            FPSCamera.GetComponent<Camera>().enabled = false;
        }

        SetUserUI();
    }

    void SetUserUI() {
        if (userNameText != null) {
            userNameText.text = photonView.Owner.NickName;
        }
    }
}
