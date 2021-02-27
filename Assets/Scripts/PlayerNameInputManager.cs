using UnityEngine;

/*
 * PlayerNameInputManager class implements player name input logic when players first log in
 */
public class PlayerNameInputManager : MonoBehaviour
{
    // For handling username entering logic
    public void SetPlayerName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Player name is empty");
            ExpoEventManager.IsNameInputTouched = false;
            return;
        }

        Debug.Log("player name: " + playerName);
        ExpoEventManager.IsNameInputTouched = true;
        ExpoEventManager.InitialName = playerName;
    }
}
