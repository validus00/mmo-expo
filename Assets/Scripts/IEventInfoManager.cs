/*
 * This interface is for handling event info related logic
 */
public interface IEventInfoManager {
    // For getting event info owner
    string EventInfoOwner { get; }
    // For opening event info panel
    void OpenEventInfo();
    // For resetting event info
    void ResetEventInfo();
    // For setting up event info
    bool SetUpEventInfo(string eventInfoUrl, string scheduleUrl, string zoomUrl);
}
