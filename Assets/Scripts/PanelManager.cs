using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour, IPanelManager {
    public GameObject boothInfoPanel;
    public GameObject boothFormPanel;
    public GameObject confirmBoothResetPanel;
    public GameObject exitEventPanel;
    public GameObject eventInfoPanel;
    public GameObject eventInfoFormPanel;
    public GameObject confirmEventInfoResetPanel;
    public Text teamNameText;
    public Text projectNameText;
    public Text descriptionText;
    public Text projectUrlText;
    public Text eventInfoUrlText;
    public Text scheduleUrlText;
    public Text zoomUrlText;
    public InputField teamNameInputField;
    public InputField projectNameInputField;
    public InputField descriptionInputField;
    public InputField projectUrlInputField;
    public InputField posterUrlInputField;
    public InputField eventInfoUrlInputField;
    public InputField scheduleUrlInputField;
    public InputField zoomUrlInputField;
    private BoothSetup __boothSetup;
    private EventInfoManager __eventInfoManager;
    public GameObject resetBoothButton;
    public GameObject resetEventInfoButton;
    public Text boothFormWarningText;
    public Text eventInfoFormWarningText;

    public void OpenProjectUrl() {
        __OpenUrlText(projectUrlText);
    }

    public void OpenEventInfoUrl() {
        __OpenUrlText(eventInfoUrlText);
    }

    public void OpenScheduleUrl() {
        __OpenUrlText(scheduleUrlText);
    }

    public void OpenZoomUrl() {
        __OpenUrlText(zoomUrlText);
    }

    private void __OpenUrlText(Text urlText) {
        if (!string.IsNullOrEmpty(urlText.text)) {
            Application.ExternalEval($"window.open(\"{__ProcessUrl(urlText.text)}\",\"_blank\")");
        }
    }

    private string __ProcessUrl(string url) {
        if (url.StartsWith("https://") || url.StartsWith("http://")) {
            return url;
        }
        return $"http://{url}";
    }

    public void CloseBoothInfoPanel() {
        boothInfoPanel.SetActive(false);
    }

    public void CloseEventInfoPanel() {
        eventInfoPanel.SetActive(false);
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
        projectUrlText.text = url;
    }

    public void OpenBoothFormPanel(BoothSetup boothSetup) {
        __boothSetup = boothSetup;
        boothFormPanel.SetActive(true);
    }

    public void SubmitBoothForm() {
        if (__boothSetup != null) {
            if (!string.IsNullOrWhiteSpace(teamNameInputField.text) &&
            !string.IsNullOrWhiteSpace(projectNameInputField.text) &&
            !string.IsNullOrWhiteSpace(descriptionInputField.text) &&
            !string.IsNullOrWhiteSpace(projectUrlInputField.text)) {
                bool isSuccessful = __boothSetup.SetUpBooth(teamNameInputField.text, projectNameInputField.text, descriptionInputField.text,
                    projectUrlInputField.text, posterUrlInputField.text);
                if (isSuccessful) {
                    CloseBoothFormPanel();
                } else {
                    boothFormWarningText.text = "This booth is already occupied";
                }
            } else {
                boothFormWarningText.text = "Please fill out all fields";
            }
        }
    }

    public void SubmitEventInfoForm() {
        if (__eventInfoManager != null) {
            if (!string.IsNullOrWhiteSpace(eventInfoUrlInputField.text) &&
            !string.IsNullOrWhiteSpace(scheduleUrlInputField.text) &&
            !string.IsNullOrWhiteSpace(zoomUrlInputField.text)) {
                bool isSuccessful = __eventInfoManager.SetUpEventInfo(eventInfoUrlInputField.text, scheduleUrlInputField.text, zoomUrlInputField.text);
                if (isSuccessful) {
                    CloseEventInfoFormPanel();
                } else {
                    eventInfoFormWarningText.text = "Event info is already set up";
                }
            } else {
                eventInfoFormWarningText.text = "Please fill out all fields";
            }
        }
    }

    public void OpenEventInfoPanel(EventInfoManager eventInfoManager, bool isOwner, string eventInfoUrl, string scheduleUrl, string zoomUrl) {
        __eventInfoManager = eventInfoManager;
        eventInfoPanel.SetActive(true);
        if (isOwner) {
            resetEventInfoButton.SetActive(true);
        } else {
            resetEventInfoButton.SetActive(false);
        }
        eventInfoUrlText.text = eventInfoUrl;
        scheduleUrlText.text = scheduleUrl;
        zoomUrlText.text = zoomUrl;
    }

    public void OpenEventInfoFormPanel(EventInfoManager eventInfoManager) {
        __eventInfoManager = eventInfoManager;
        eventInfoFormPanel.SetActive(true);
    }

    public void CloseEventInfoFormPanel() {
        eventInfoFormWarningText.text = string.Empty;
        eventInfoFormPanel.SetActive(false);
    }

    public void CloseBoothFormPanel() {
        boothFormWarningText.text = string.Empty;
        boothFormPanel.SetActive(false);
    }

    public bool IsAnyPanelActive() {
        return boothInfoPanel.activeSelf || boothFormPanel.activeSelf || confirmBoothResetPanel.activeSelf
            || exitEventPanel.activeSelf || eventInfoPanel.activeSelf || eventInfoFormPanel.activeSelf
            || confirmEventInfoResetPanel.activeSelf;
    }

    public void OpenConfirmBoothResetPanel() {
        confirmBoothResetPanel.SetActive(true);
    }

    public void OpenConfirmEventInfoResetPanel() {
        confirmEventInfoResetPanel.SetActive(true);
    }

    public void CloseConfirmBoothResetPanel() {
        confirmBoothResetPanel.SetActive(false);
    }

    public void CloseConfirmEventInfoResetPanel() {
        confirmEventInfoResetPanel.SetActive(false);
    }

    public void ResetBooth() {
        __boothSetup.ResetBooth();
        CloseConfirmBoothResetPanel();
        CloseBoothInfoPanel();
    }

    public void ResetEventInfo() {
        __eventInfoManager.ResetEventInfo();
        CloseConfirmEventInfoResetPanel();
        CloseEventInfoPanel();
    }

    public void ToggleExitEventPanel() {
        exitEventPanel.SetActive(!exitEventPanel.activeSelf);
    }
}
