using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BoothManager : MonoBehaviour {
    public GameObject boothInfoPanel;
    public GameObject boothFormPanel;
    public Text teamNameText;
    public Text projectNameText;
    public Text descriptionText;
    public Text urlText;
    public InputField teamNameInputField;
    public InputField projectNameInputField;
    public InputField descriptionInputField;
    public InputField urlInputField;
    private BoothSetup __boothSetup;
    public GameObject resetBoothButton;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OpenURL() {
        if (urlText != null && !string.IsNullOrEmpty(urlText.text)) {
            Application.ExternalEval($"window.open(\"{urlText.text}\",\"_blank\")");
        }
    }

    public void CloseBoothInfoPanel() {
        boothInfoPanel.SetActive(false);
    }

    public void OpenBoothInfoPanel(BoothSetup boothSetup, bool isOwner, string projectName, string teamName,
        string description, string url) {
        __boothSetup = boothSetup;
        boothInfoPanel.SetActive(true);
        if (isOwner) {
            resetBoothButton.SetActive(true);
        } else {
            resetBoothButton.SetActive(false);
        }
        teamNameText.text = teamName;
        projectNameText.text = projectName;
        descriptionText.text = description;
        urlText.text = url;
    }

    public void OpenBoothFormPanel(BoothSetup boothSetup) {
        __boothSetup = boothSetup;
        boothFormPanel.SetActive(true);
    }

    public void SubmitForm() {
        if (__boothSetup != null && !string.IsNullOrWhiteSpace(teamNameInputField.text) &&
            !string.IsNullOrWhiteSpace(projectNameInputField.text) &&
            !string.IsNullOrWhiteSpace(descriptionInputField.text) &&
            !string.IsNullOrWhiteSpace(urlInputField.text)) {
            __boothSetup.SetUpBooth(teamNameInputField.text, projectNameInputField.text, descriptionInputField.text,
                urlInputField.text);
            boothFormPanel.SetActive(false);
        }
    }

    public void CloseBoothFormPanel() {
        boothFormPanel.SetActive(false);
    }

    public bool IsAnyBoothPanelActive() {
        return boothInfoPanel.activeSelf || boothFormPanel.activeSelf;
    }

    public void ResetBooth() {
        __boothSetup.ResetBooth();
        boothInfoPanel.SetActive(false);
    }
}
