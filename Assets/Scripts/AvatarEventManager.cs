using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarEventManager : MonoBehaviour {
    // Save previously selected avatar button game object
    private GameObject __prevSelectedAvatar;

    // Hold references to the avatar buttons
    public GameObject FemaleAvatarButton;
    public GameObject MaleAvatarButton;

    private ColorBlock __buttonColors;

    // Used to detect if an avatar button was selected, if not, keep focus on previously selected
    // Ex: Clicking on background image will not defocus avatar button
    void Update() {
        if (EventSystem.current.currentSelectedGameObject == null && __prevSelectedAvatar != null) {
            if (__prevSelectedAvatar.gameObject.activeSelf && __prevSelectedAvatar.GetComponent<Button>() 
                != null && __prevSelectedAvatar.GetComponent<Button>().interactable) {
                EventSystem.current.SetSelectedGameObject(__prevSelectedAvatar);
            }
        } else {
            // Check to see if the object pressed is an avatar button
            if (EventSystem.current.currentSelectedGameObject == FemaleAvatarButton
                || EventSystem.current.currentSelectedGameObject == MaleAvatarButton) {
                if (__prevSelectedAvatar != null) {
                    // Reset the previously selected avatar button to white
                    __buttonColors = __prevSelectedAvatar.GetComponent<Button>().colors;
                    __buttonColors.normalColor = Color.white;
                    __prevSelectedAvatar.GetComponent<Button>().colors = __buttonColors;
                    __prevSelectedAvatar = EventSystem.current.currentSelectedGameObject;
                } else {
                    __prevSelectedAvatar = EventSystem.current.currentSelectedGameObject;
                }
            } else {
                // If another object is pressed, keep the previous avatar button looking like it was selected
                if (__prevSelectedAvatar != null) {
                    __buttonColors = __prevSelectedAvatar.GetComponent<Button>().colors;
                    __buttonColors.normalColor = __prevSelectedAvatar.GetComponent<Button>().colors.selectedColor;
                    __prevSelectedAvatar.GetComponent<Button>().colors = __buttonColors;
                }
            }
            
        }
    }
}
