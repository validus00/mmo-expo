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
        if (PlayerPrefs.HasKey("myAvatar")) {
            mySelectedAvatar = PlayerPrefs.GetInt("myAvatar");
        } else {
            mySelectedAvatar = 0;
            PlayerPrefs.SetInt("myAvatar", mySelectedAvatar);
        }
    }
}
