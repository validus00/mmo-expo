using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour, IInputFieldHandler
{
    private InputField _inputField;

    // Start is called before the first frame update
    void Start()
    {
        _inputField = GetComponent<InputField>();
    }

    public bool IsFocused()
    {
        return _inputField.isFocused;
    }
}
