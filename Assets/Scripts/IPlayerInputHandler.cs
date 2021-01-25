using UnityEngine;

public interface IPlayerInputHandler {
    float GetLookInputsHorizontal();
    float GetLookInputsVertical();
    Vector3 GetMoveInput();
    bool GetRightClickInputHeld();
}