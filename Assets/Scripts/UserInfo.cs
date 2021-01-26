using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour {
    public static UserInfo info;
    public int mySelectedAvatar;

    private void OnEnable() {
        if (UserInfo.info == null) {
            UserInfo.info = this;
        } else {
            if (UserInfo.info != this) {
                Destroy(UserInfo.info.gameObject);
                UserInfo.info = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        if (PlayerPrefs.HasKey(GameConstants.k_MyAvatar)) {
            mySelectedAvatar = PlayerPrefs.GetInt(GameConstants.k_MyAvatar);
        } else {
            mySelectedAvatar = 0;
            PlayerPrefs.SetInt(GameConstants.k_MyAvatar, mySelectedAvatar);
        }
    }
}
