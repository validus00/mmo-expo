using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarEventManager : MonoBehaviour {
    // Save previously selected avatar button game object
    private GameObject __prevSelectedAvatar;

    // Used to detect if an avatar button was selected, if not, keep focus on previously selected
    // Ex: Clicking on background image will not defocus avatar button
    void Update() {
        if (EventSystem.current.currentSelectedGameObject == null) {
            if (__prevSelectedAvatar.gameObject.activeSelf && __prevSelectedAvatar.GetComponent<Button>() 
                != null && __prevSelectedAvatar.GetComponent<Button>().interactable) {
                EventSystem.current.SetSelectedGameObject(__prevSelectedAvatar);
            }
        } else {
            __prevSelectedAvatar = EventSystem.current.currentSelectedGameObject;
        }
    }
}
