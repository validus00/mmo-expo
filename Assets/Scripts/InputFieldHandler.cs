using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour, IInputFieldHandler {
    private InputField inputField;

    // Start is called before the first frame update
    void Start() {
        inputField = GetComponent<InputField>();
    }

    // Returns whether InputField is in focus or not
    public bool isFocused() {
        return inputField.isFocused;
    }
}
