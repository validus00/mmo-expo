using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour, IPanelManager
{
    public GameObject BoothInfoPanel;
    public GameObject BoothFormPanel;
    public GameObject ConfirmBoothResetPanel;
    public GameObject ExitEventPanel;
    public GameObject EventInfoPanel;
    public GameObject EventInfoFormPanel;
    public GameObject ConfirmEventInfoResetPanel;
    public GameObject ExpoEventManager;
    public Text TeamNameText;
    public Text ProjectNameText;
    public Text DescriptionText;
    public Text ProjectUrlText;
    public Text EventInfoUrlText;
    public Text ScheduleUrlText;
    public Text ZoomUrlText;
    public Text PassCodeText;
    public InputField TeamNameInputField;
    public InputField ProjectNameInputField;
    public InputField DescriptionInputField;
    public InputField ProjectUrlInputField;
    public InputField PosterUrlInputField;
    public InputField EventInfoUrlInputField;
    public InputField ScheduleUrlInputField;
    public InputField ZoomUrlInputField;
    private BoothSetup _boothSetup;
    private EventInfoManager _eventInfoManager;
    public GameObject ResetBoothButton;
    public GameObject ResetEventInfoButton;
    public Text BoothFormWarningText;
    public Text EventInfoFormWarningText;
    private readonly Regex _regex = new Regex("^http(s)?://", RegexOptions.IgnoreCase);

    void Start()
    {
        PassCodeText.text = ExpoEventManager.GetComponent<ExpoEventManager>().PassCode;
    }

    public void OpenProjectUrl()
    {
        OpenUrlText(ProjectUrlText);
    }

    public void OpenEventInfoUrl()
    {
        OpenUrlText(EventInfoUrlText);
    }

    public void OpenScheduleUrl()
    {
        OpenUrlText(ScheduleUrlText);
    }

    public void OpenZoomUrl()
    {
        OpenUrlText(ZoomUrlText);
    }

    private void OpenUrlText(Text urlText)
    {
        if (!string.IsNullOrEmpty(urlText.text))
        {
#pragma warning disable 618
            Application.ExternalEval($"window.open(\"{ProcessUrl(urlText.text)}\",\"_blank\")");
#pragma warning restore 618
        }
    }

    private string ProcessUrl(string url)
    {
        if (_regex.IsMatch(url))
        {
            return url;
        }
        return $"http://{url}";
    }

    public void CloseBoothInfoPanel()
    {
        BoothInfoPanel.SetActive(false);
    }

    public void CloseEventInfoPanel()
    {
        EventInfoPanel.SetActive(false);
    }

    public void OpenBoothInfoPanel(BoothSetup boothSetup, bool isOwner, string projectName, string teamName,
        string description, string url)
    {
        _boothSetup = boothSetup;
        BoothInfoPanel.SetActive(true);
        if (isOwner)
        {
            ResetBoothButton.SetActive(true);
        }
        else
        {
            ResetBoothButton.SetActive(false);
        }
        TeamNameText.text = teamName;
        ProjectNameText.text = projectName;
        DescriptionText.text = description;
        ProjectUrlText.text = url;
    }

    public void OpenBoothFormPanel(BoothSetup boothSetup)
    {
        _boothSetup = boothSetup;
        BoothFormPanel.SetActive(true);
    }

    public void SubmitBoothForm()
    {
        if (_boothSetup != null)
        {
            if (!string.IsNullOrWhiteSpace(TeamNameInputField.text) &&
            !string.IsNullOrWhiteSpace(ProjectNameInputField.text) &&
            !string.IsNullOrWhiteSpace(DescriptionInputField.text) &&
            !string.IsNullOrWhiteSpace(ProjectUrlInputField.text))
            {
                bool isSuccessful = _boothSetup.SetUpBooth(TeamNameInputField.text, ProjectNameInputField.text, DescriptionInputField.text,
                    ProjectUrlInputField.text, PosterUrlInputField.text);
                if (isSuccessful)
                {
                    CloseBoothFormPanel();
                }
                else
                {
                    BoothFormWarningText.text = "This booth is already occupied";
                }
            }
            else
            {
                BoothFormWarningText.text = "Please fill out all fields";
            }
        }
    }

    public void SubmitEventInfoForm()
    {
        if (_eventInfoManager != null)
        {
            if (!string.IsNullOrWhiteSpace(EventInfoUrlInputField.text) &&
            !string.IsNullOrWhiteSpace(ScheduleUrlInputField.text) &&
            !string.IsNullOrWhiteSpace(ZoomUrlInputField.text))
            {
                bool isSuccessful = _eventInfoManager.SetUpEventInfo(EventInfoUrlInputField.text, ScheduleUrlInputField.text, ZoomUrlInputField.text);
                if (isSuccessful)
                {
                    CloseEventInfoFormPanel();
                }
                else
                {
                    EventInfoFormWarningText.text = "Event info is already set up";
                }
            }
            else
            {
                EventInfoFormWarningText.text = "Please fill out all fields";
            }
        }
    }

    public void OpenEventInfoPanel(EventInfoManager eventInfoManager, bool isOwner, string eventInfoUrl, string scheduleUrl, string zoomUrl)
    {
        this._eventInfoManager = eventInfoManager;
        EventInfoPanel.SetActive(true);
        if (isOwner)
        {
            ResetEventInfoButton.SetActive(true);
        }
        else
        {
            ResetEventInfoButton.SetActive(false);
        }
        EventInfoUrlText.text = eventInfoUrl;
        ScheduleUrlText.text = scheduleUrl;
        ZoomUrlText.text = zoomUrl;
    }

    public void OpenEventInfoFormPanel(EventInfoManager eventInfoManager)
    {
        this._eventInfoManager = eventInfoManager;
        EventInfoFormPanel.SetActive(true);
    }

    public void CloseEventInfoFormPanel()
    {
        EventInfoFormWarningText.text = string.Empty;
        EventInfoFormPanel.SetActive(false);
    }

    public void CloseBoothFormPanel()
    {
        BoothFormWarningText.text = string.Empty;
        BoothFormPanel.SetActive(false);
    }

    public bool IsAnyPanelActive()
    {
        return BoothInfoPanel.activeSelf || BoothFormPanel.activeSelf || ConfirmBoothResetPanel.activeSelf
            || ExitEventPanel.activeSelf || EventInfoPanel.activeSelf || EventInfoFormPanel.activeSelf
            || ConfirmEventInfoResetPanel.activeSelf;
    }

    public void OpenConfirmBoothResetPanel()
    {
        ConfirmBoothResetPanel.SetActive(true);
    }

    public void OpenConfirmEventInfoResetPanel()
    {
        ConfirmEventInfoResetPanel.SetActive(true);
    }

    public void CloseConfirmBoothResetPanel()
    {
        ConfirmBoothResetPanel.SetActive(false);
    }

    public void CloseConfirmEventInfoResetPanel()
    {
        ConfirmEventInfoResetPanel.SetActive(false);
    }

    public void ResetBooth()
    {
        _boothSetup.ResetBooth();
        CloseConfirmBoothResetPanel();
        CloseBoothInfoPanel();
    }

    public void ResetEventInfo()
    {
        _eventInfoManager.ResetEventInfo();
        CloseConfirmEventInfoResetPanel();
        CloseEventInfoPanel();
    }

    public void ToggleExitEventPanel()
    {
        ExitEventPanel.SetActive(!ExitEventPanel.activeSelf);
    }
}
