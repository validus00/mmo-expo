using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerNameInputManager : MonoBehaviour
{
    public void SetPlayerName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Player name is empty");
            ExpoEventManager.isNameInputTouched = false;
            return;
        }

        Debug.Log("player name: " + playerName);
        ExpoEventManager.isNameInputTouched = true;
        ExpoEventManager.initialName = playerName;
    }
}
