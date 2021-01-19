using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviour {

    private PhotonView __photonView;
    public int avatarValue;
    public GameObject myAvatar;
    // Start is called before the first frame update
    void Start() {
        __photonView = GetComponent<PhotonView>();
        if (__photonView.IsMine) {
            __photonView.RPC("addCharacterRPC", RpcTarget.AllBuffered, UserInfo.info.mySelectedAvatar);
        }
    }

    [PunRPC]
    void addCharacterRPC(int avatar) {
        avatarValue = avatar;
    }
}
