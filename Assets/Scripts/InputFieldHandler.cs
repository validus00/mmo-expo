using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour, IInputFieldHandler {
    private InputField inputField;

    // Start is called before the first frame update
    void Start() {
        inputField = GetComponent<InputField>();
    }

    public bool isFocused() {
        return inputField.isFocused;
    }
}
