using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class RoomListManager : MonoBehaviour, IPointerClickHandler {
    // Dropdown object reference
    public Dropdown dropdown;
    // List of indexes to disable from dropdown
    public List<int> indexesToDisable = new List<int>();
    // Channel box from chat reference
    public InputField channelBox;
    // Hold the user's name
    private string __name;
    // Default placeholder text for dropdown
    private readonly string __defaultText = "Users in Room";
    // String concatenated to identifer user in room list
    private readonly string __selfIndicatorString = " (You)";
    // List to hold user names
    private readonly List<string> __usernames = new List<string>();

    void Awake() {
        __name = PhotonNetwork.NickName;
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });

    }

    void Update() {
        // Reset the user's name if identifier holds default
        if (ExpoEventManager.isNameUpdated && __name.StartsWith("User")) {
            __name = PhotonNetwork.NickName;

        }
        PopulateDropdown();
    }

    // Listener to populate the channel box text and reset the selected option to default
    void DropdownItemSelected(Dropdown dropdown) {
        if (!dropdown.options[dropdown.value].text.Equals(__defaultText) &&
            !dropdown.options[dropdown.value].text.Equals(__name + __selfIndicatorString)) {
            channelBox.text = dropdown.options[dropdown.value].text;
        }

        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    // Prevent the selection of the default text (index 0)
    // Referenced: https://stackoverflow.com/questions/55297626/disable-an-options-in-a-dropdown-unity
    public void OnPointerClick(PointerEventData eventData) {
        var dropDownList = GetComponentInChildren<Canvas>();
        if (!dropDownList) return;

        // If the dropdown was opened find the options toggles
        var toogles = dropDownList.GetComponentsInChildren<Toggle>(true);

        // the first item will always be a template item from the dropdown we have to ignore
        // so we start at one and all options indexes have to be 1 based
        for (var i = 1; i < toogles.Length; i++) {
            // disable buttons if their 0-based index is in indexesToDisable
            // the first item will always be a template item from the dropdown
            // so in order to still have 0 based indexes for the options here we use i-1
            toogles[i].interactable = !indexesToDisable.Contains(i - 1);
        }
        PopulateDropdown();
    }

    // Populate the dropdown with the users in the room
    public void PopulateDropdown() {
        // Clear the list and dropdown and add names and default text to list
        __usernames.Clear();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = __defaultText });
        foreach (Player player in PhotonNetwork.PlayerList) {
            if (player.NickName.Equals(__name)) {
                // Keep the local user at the top of the list
                __usernames.Insert(0, PhotonChatHandler.RemoveRoomName(player.NickName) + __selfIndicatorString);
            } else {
                __usernames.Add(PhotonChatHandler.RemoveRoomName(player.NickName));
            }
        }
        // Use the list to add the options to dropdown
        foreach (var name in __usernames) {
            dropdown.options.Add(new Dropdown.OptionData() { text = name });
        }
    }
}
