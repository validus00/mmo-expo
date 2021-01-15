using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ExpoEventManager : MonoBehaviourPunCallbacks {

    [SerializeField]
    GameObject userPrefab;

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.IsConnected) {
            if (userPrefab != null) {
                int randomPoint = Random.Range(-20, 20);
                PhotonNetwork.Instantiate(userPrefab.name, new Vector3(randomPoint, 0, randomPoint), Quaternion.identity);
            }

        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
