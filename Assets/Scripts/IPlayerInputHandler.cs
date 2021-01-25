using UnityEngine;

public interface IPlayerInputHandler {
    // Returns value of any horizontal mouse movement
    float GetLookInputsHorizontal();
    // Returns value of any vertical mouse movement
    float GetLookInputsVertical();
    // Returns a vector corresponding to the user's keyboard inputs
    Vector3 GetMoveInput();
    // Returns whether the return key is pressed
    bool GetReturnKey();
    // Returns whether the right mouse button is held down
    bool GetRightClickInputHeld();
}