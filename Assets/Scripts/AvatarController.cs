using UnityEngine;

public class AvatarController : MonoBehaviour {
    public void OnClickAvatarSelection(int avatar) {
        ExpoEventManager.mySelectedAvatar = avatar;
    }
}
