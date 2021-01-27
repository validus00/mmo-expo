using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour, IPanelManager {
    public GameObject boothInfoPanel;
    public GameObject boothFormPanel;
    public GameObject confirmBoothResetPanel;
    public GameObject exitEventPanel;
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
    public Text warningText;

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
        if (__boothSetup != null) {
            if (!string.IsNullOrWhiteSpace(teamNameInputField.text) &&
            !string.IsNullOrWhiteSpace(projectNameInputField.text) &&
            !string.IsNullOrWhiteSpace(descriptionInputField.text) &&
            !string.IsNullOrWhiteSpace(urlInputField.text)) {
                __boothSetup.SetUpBooth(teamNameInputField.text, projectNameInputField.text, descriptionInputField.text,
                    urlInputField.text);
                CloseBoothFormPanel();
            } else {
                warningText.text = "Please fill out all fields";
            }
        }
    }

    public void CloseBoothFormPanel() {
        warningText.text = string.Empty;
        boothFormPanel.SetActive(false);
    }

    public bool IsAnyPanelActive() {
        return boothInfoPanel.activeSelf || boothFormPanel.activeSelf || confirmBoothResetPanel.activeSelf || exitEventPanel.activeSelf;
    }

    public void OpenConfirmBoothResetPanel() {
        confirmBoothResetPanel.SetActive(true);
    }

    public void CloseConfirmBoothResetPanel() {
        confirmBoothResetPanel.SetActive(false);
    }

    public void ResetBooth() {
        __boothSetup.ResetBooth();
        CloseConfirmBoothResetPanel();
        CloseBoothInfoPanel();
    }

    public void ToggleExitEventPanel() {
        exitEventPanel.SetActive(!exitEventPanel.activeSelf);
    }
}
