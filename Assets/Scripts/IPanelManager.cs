/*
 * This interface is for handling panel related logic
 */
public interface IPanelManager
{
    // For closing booth form panel
    void CloseBoothFormPanel();
    // For closing booth info panel
    void CloseBoothInfoPanel();
    // For closing confirm booth reset panel
    void CloseConfirmBoothResetPanel();
    // For closing confirm event info reset panel
    void CloseConfirmEventInfoResetPanel();
    // For closing event info form panel
    void CloseEventInfoFormPanel();
    // For closing event info panel
    void CloseEventInfoPanel();
    // For determining whether any panels are active
    bool IsAnyPanelActive();
    // For opening booth form panel
    void OpenBoothFormPanel(BoothSetup boothSetup);
    // For opening booth info panel
    void OpenBoothInfoPanel(BoothSetup boothSetup, bool isOwner, string projectName, string teamName,
        string description, string url);
    // For opening confirm booth reset panel
    void OpenConfirmBoothResetPanel();
    // For opening confirm booth reset panel
    void OpenConfirmEventInfoResetPanel();
    // For opening event info form panel
    void OpenEventInfoFormPanel(EventInfoManager eventInfoManager);
    // For opening event info panel
    void OpenEventInfoPanel(EventInfoManager eventInfoManager, bool isOwner, string eventInfoUrl, string scheduleUrl,
        string zoomUrl);
    // For opening event info URL
    void OpenEventInfoUrl();
    // For opening project URL
    void OpenProjectUrl();
    // For opening schedule URL
    void OpenScheduleUrl();
    // For opening zoom URL
    void OpenZoomUrl();
    // For resetting booth information
    void ResetBooth();
    // For resetting event information
    void ResetEventInfo();
    // For submitting booth signup form
    void SubmitBoothForm();
    // For submitting event info form
    void SubmitEventInfoForm();
    // For toggling exit event panel on or off
    void ToggleExitEventPanel();
}
