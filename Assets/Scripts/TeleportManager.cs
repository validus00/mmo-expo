using UnityEngine;
using Photon.Pun;

public class TeleportManager : MonoBehaviour {
    public Transform teleportTarget;

    public void OnTriggerEnter(Collider other) {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (other.gameObject.name.Equals("MyUser")) {
            // Turn off the character controller so that the user can be teleported
            controller.enabled = false;
            other.GetComponent<PhotonView>().RPC("CharacterControllerToggle", RpcTarget.AllBuffered, controller.enabled);
            other.transform.position = teleportTarget.transform.position;
        }
    }
}
