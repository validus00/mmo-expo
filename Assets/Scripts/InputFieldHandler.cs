using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour, IInputFieldHandler
{
    private InputField __inputField;

    // Start is called before the first frame update
    void Start()
    {
        __inputField = GetComponent<InputField>();
    }

    public bool isFocused()
    {
        return __inputField.isFocused;
    }
}
