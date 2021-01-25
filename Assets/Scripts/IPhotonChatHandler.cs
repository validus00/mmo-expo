using System.Collections.Generic;

public interface IPhotonChatHandler {
    void EnterChannel(string channelName);
    List<Message> GetNewMessages();
    void LeaveChannel(string channelName);
    void Initialize();
    void InitializeChannels(string[] channels);
    bool IsConnected();
    void MaintainService();
    void SendChannelMessage(string channelName, string message);
}