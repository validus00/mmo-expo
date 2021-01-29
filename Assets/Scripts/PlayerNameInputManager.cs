using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNameInputManager : MonoBehaviour
{
    public void SetPlayerName(string playerName) {
        if (string.IsNullOrEmpty(playerName)) {
            Debug.Log("Player name is empty");
            LaunchManager.isNameInputTouched = false;
            return;
        }

        Debug.Log("player name: " + playerName);
        LaunchManager.isNameInputTouched = true;
        LaunchManager.initialName = playerName;
    }
}
