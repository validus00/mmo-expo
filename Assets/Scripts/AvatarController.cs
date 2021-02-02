using UnityEngine;

public class AvatarController : MonoBehaviour {
    public void OnClickAvatarSelection(int avatar) {
        if (UserInfo.info != null) {
            UserInfo.info.mySelectedAvatar = avatar;
        }
    }
}
