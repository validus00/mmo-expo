using System.Collections.Generic;

public interface IPhotonChatHandler {
    // For entering a specific channel
    void EnterChannel(string channelName);
    // Returns new messages and empty list of messages
    List<Message> GetNewMessages();
    // For leaving a specific channel
    void LeaveChannel(string channelName);
    // For initial setup
    void Initialize();
    // For setting up the initial channels when first connecting to service
    void InitializeChannels(string[] channels);
    // For determining if connected to Photon Chat
    bool IsConnected();
    // For maintaining connection to Photon Chat
    void MaintainService();
    // For sending message to a specific channel
    void SendChannelMessage(string channelName, string message);
}