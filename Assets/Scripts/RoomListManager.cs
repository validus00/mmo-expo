using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviour, IPointerClickHandler
{
    // Dropdown object reference
    public Dropdown Dropdown;
    // List of indexes to disable from dropdown
    public List<int> IndexesToDisable = new List<int>();
    // Channel box from chat reference
    public InputField ChannelBox;
    // Hold the user's name with concatenated with room name
    private string _name;
    // Hold the user's name without room name
    private string _nameTrimmed;
    // Default placeholder text for dropdown
    private const string K_defaultText = "Users in Room";
    // String concatenated to identifer user in room list
    private const string K_selfIndicatorString = " (You)";
    // List to hold user names
    private readonly List<string> _usernames = new List<string>();

    void Awake()
    {
        _name = PhotonNetwork.NickName;
        Dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(Dropdown); });

    }

    void Update()
    {
        // Reset the user's name if identifier holds default
        if (ExpoEventManager.isNameUpdated && _name.StartsWith("User"))
        {
            _name = PhotonNetwork.NickName;
            _nameTrimmed = PhotonChatHandler.RemoveRoomName(_name);
        }
        PopulateDropdown();
    }

    // Listener to populate the channel box text and reset the selected option to default
    void DropdownItemSelected(Dropdown dropdown)
    {
        if (!dropdown.options[dropdown.value].text.Equals(K_defaultText) &&
            !dropdown.options[dropdown.value].text.Equals(_nameTrimmed + K_selfIndicatorString))
        {
            ChannelBox.text = dropdown.options[dropdown.value].text;
        }

        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    // Prevent the selection of the default text (index 0)
    // Referenced: https://stackoverflow.com/questions/55297626/disable-an-options-in-a-dropdown-unity
    public void OnPointerClick(PointerEventData eventData)
    {
        var dropDownList = GetComponentInChildren<Canvas>();
        if (!dropDownList) return;

        // If the dropdown was opened find the options toggles
        var toogles = dropDownList.GetComponentsInChildren<Toggle>(true);

        // the first item will always be a template item from the dropdown we have to ignore
        // so we start at one and all options indexes have to be 1 based
        for (var i = 1; i < toogles.Length; i++)
        {
            // disable buttons if their 0-based index is in indexesToDisable
            // the first item will always be a template item from the dropdown
            // so in order to still have 0 based indexes for the options here we use i-1
            toogles[i].interactable = !IndexesToDisable.Contains(i - 1);
        }
        PopulateDropdown();
    }

    // Populate the dropdown with the users in the room
    public void PopulateDropdown()
    {
        // Clear the list and dropdown and add names and default text to list
        _usernames.Clear();
        Dropdown.options.Clear();
        Dropdown.options.Add(new Dropdown.OptionData() { text = K_defaultText });
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName.Equals(_name))
            {
                // Keep the local user at the top of the list
                _usernames.Insert(0, PhotonChatHandler.RemoveRoomName(player.NickName) + K_selfIndicatorString);
            }
            else
            {
                _usernames.Add(PhotonChatHandler.RemoveRoomName(player.NickName));
            }
        }
        // Use the list to add the options to dropdown
        foreach (var name in _usernames)
        {
            Dropdown.options.Add(new Dropdown.OptionData() { text = name });
        }
    }
}
