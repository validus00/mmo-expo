using UnityEngine;

public class UserInfo : MonoBehaviour {
    public static UserInfo info;
    public int mySelectedAvatar;

    void Awake() {
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
}
