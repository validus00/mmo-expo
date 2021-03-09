using UnityEngine;

/*
 * This interface is for handling user movement and camera input changes
 */
public interface IPlayerInputHandler
{
    // Returns a vector corresponding to the user's keyboard inputs for flying
    Vector3 GetFlyingInput(Vector3 vector);
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
    // Returns whether the tab key is pressed
    bool GetTabKey();
}
