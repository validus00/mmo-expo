using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour {
    public void OnClickAvatarSelection(int avatar) {
        if (UserInfo.info != null) {
            UserInfo.info.mySelectedAvatar = avatar;
            PlayerPrefs.SetInt(GameConstants.k_MyAvatar, avatar);
        }
    }
}
