using UnityEngine;

public class PlayerNameInputManager : MonoBehaviour
{
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
