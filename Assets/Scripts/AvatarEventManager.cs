using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarEventManager : MonoBehaviour
{
    // Save previously selected avatar button game object
    private GameObject _prevSelectedAvatar;
    // Hold references to the avatar buttons
    public GameObject FemaleAvatarButton;
    public GameObject MaleAvatarButton;
    // For hiding unselected avatar text after button is pressed
    public GameObject UnselectedAvatarText;
    // For holding current button color sets
    private ColorBlock _buttonColors;

    // Used to detect if an avatar button was selected, if not, keep focus on previously selected
    // Ex: Clicking on background image will not defocus avatar button
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && _prevSelectedAvatar != null)
        {
            if (_prevSelectedAvatar.gameObject.activeSelf && _prevSelectedAvatar.GetComponent<Button>()
                != null && _prevSelectedAvatar.GetComponent<Button>().interactable)
            {
                EventSystem.current.SetSelectedGameObject(_prevSelectedAvatar);
            }
        }
        else
        {
            // Check to see if the object pressed is an avatar button
            if (EventSystem.current.currentSelectedGameObject == FemaleAvatarButton
                || EventSystem.current.currentSelectedGameObject == MaleAvatarButton)
            {
                // Button has been pressed, so hide unselected avatar text
                UnselectedAvatarText.SetActive(false);

                if (_prevSelectedAvatar != null)
                {
                    // Reset the previously selected avatar button to white
                    _buttonColors = _prevSelectedAvatar.GetComponent<Button>().colors;
                    _buttonColors.normalColor = Color.white;
                    _prevSelectedAvatar.GetComponent<Button>().colors = _buttonColors;
                    _prevSelectedAvatar = EventSystem.current.currentSelectedGameObject;
                }
                else
                {
                    _prevSelectedAvatar = EventSystem.current.currentSelectedGameObject;
                }
            }
            else
            {
                // If another object is pressed, keep the previous avatar button looking like it was selected
                if (_prevSelectedAvatar != null)
                {
                    _buttonColors = _prevSelectedAvatar.GetComponent<Button>().colors;
                    _buttonColors.normalColor = _prevSelectedAvatar.GetComponent<Button>().colors.selectedColor;
                    _prevSelectedAvatar.GetComponent<Button>().colors = _buttonColors;
                }
            }

        }
    }
}
