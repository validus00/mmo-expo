using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportManager : MonoBehaviour {
    public Transform teleportTarget;

    public void OnTriggerEnter(Collider other) {
        // Create a reference for the character controller
        CharacterController controller = other.GetComponent<CharacterController>();

        // Turn off the character controller so that the user can be teleported
        controller.enabled = false;

        // Teleport the user
        other.transform.position = teleportTarget.transform.position;

        // When character controller is re-enabled, it will grab the new position
        controller.enabled = true;
    }
}
