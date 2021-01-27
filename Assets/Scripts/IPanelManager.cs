/*
 * This interface is for handling panel related logic
 */
public interface IPanelManager {
    // For closing booth form panel
    void CloseBoothFormPanel();
    // For closing booth info panel
    void CloseBoothInfoPanel();
    // For closing confirm booth reset panel
    void CloseConfirmBoothResetPanel();
    // For determining whether any panels are active
    bool IsAnyPanelActive();
    // For opening booth form panel
    void OpenBoothFormPanel(BoothSetup boothSetup);
    // For opening booth info panel
    void OpenBoothInfoPanel(BoothSetup boothSetup, bool isOwner, string projectName, string teamName, string description, string url);
    // For opening confirm booth reset panel
    void OpenConfirmBoothResetPanel();
    // For opening project URL
    void OpenURL();
    // For resetting booth information
    void ResetBooth();
    // For submitting booth signup form
    void SubmitForm();
    // For toggling exit event panel on or off
    void ToggleExitEventPanel();
}