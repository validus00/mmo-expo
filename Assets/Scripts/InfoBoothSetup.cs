using Photon.Pun;
using UnityEngine;

public class InfoBoothSetup : MonoBehaviourPunCallbacks {
    private Camera __camera;
    private EventInfoManager __eventInfoManager;

    void Start() {
        __eventInfoManager = GameObject.Find(GameConstants.k_EventInfoManager).GetComponent<EventInfoManager>();
    }

    // This is called when the user first enters a booth area
    void OnTriggerEnter(Collider other) {
        if (__camera == null) {
            __camera = GameObject.Find(GameConstants.k_Camera).GetComponent<Camera>();
            GetComponentInChildren<Canvas>().worldCamera = __camera;
        }

        if (other.name == GameConstants.k_MyUser) {
            EventInfoManager.isUserInInfoBooth = true;
        }
    }

    // This is called when the user leaves a booth area
    void OnTriggerExit(Collider other) {
        if (other.name == GameConstants.k_MyUser) {
            EventInfoManager.isUserInInfoBooth = false;
        }
    }

    public void OpenEventInfo() {
        __eventInfoManager.OpenEventInfo();
    }
}
