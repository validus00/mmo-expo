﻿using UnityEngine;
using Photon.Pun;
using TMPro;

public class TeleportManager : MonoBehaviour {
    public Transform teleportTarget;
    public TextMeshProUGUI destinationText;
    private ChatManager __chatManager;
    [SerializeField]
    private string __currentChannelName;

    void Start() {
        __chatManager = GameObject.Find(GameConstants.k_ExpoEventManager).GetComponent<ChatManager>();
    }

    public void OnTriggerEnter(Collider other) {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (other.gameObject.name.Equals("MyUser")) {
            // Turn off the character controller so that the user can be teleported
            controller.enabled = false;
            other.GetComponent<PhotonView>().RPC("CharacterControllerToggle", RpcTarget.AllBuffered, controller.enabled);
            other.transform.position = teleportTarget.transform.position;
            __changeChannel();
        }
    }

    private void __changeChannel() {
        string newChannelName = destinationText.text;
        __chatManager.UpdateChannel(newChannelName, ChatManager.ChannelType.hallChannel);
        __chatManager.EnterChannel(newChannelName);
        __chatManager.LeaveChannel(__currentChannelName);
    }
}
