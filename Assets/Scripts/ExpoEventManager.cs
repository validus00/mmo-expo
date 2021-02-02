using UnityEngine;
using Photon.Pun;

public class ExpoEventManager : MonoBehaviourPunCallbacks {

    [SerializeField]
    GameObject[] listOfAvatars;

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.IsConnected) {
            int avatarPick = PlayerPrefs.GetInt(GameConstants.k_MyAvatar);
            GameObject user = listOfAvatars[avatarPick];
            if (user != null) {
                int randomPoint = Random.Range(-20, 20); 
                PhotonNetwork.Instantiate(user.name, new Vector3(randomPoint, 0, randomPoint), Quaternion.identity);
            }

        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel("EventLauncherScene");
    }
}
