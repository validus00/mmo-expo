using System.Collections.Generic;

/*
 * This interface is for handling and abstracting Photon Chat functionality
 */
public interface IPhotonChatHandler {
    // For entering a specific channel
    void EnterChannel(string channelName);
    // Returns new messages and empty list of messages
    List<Message> GetNewMessages();
    // For leaving a specific channel
    void LeaveChannel(string channelName);
    // For initial setup
    void InitializeChannelNames(string[] channels);
    // For determining if connected to Photon Chat
    bool IsConnected();
    // For maintaining connection to Photon Chat
    void MaintainService();
    // For sending message to a specific channel
    void SendChannelMessage(string channelName, string message);
    // FOr sending private messages to a specific user
    void SendPrivateMessage(string channelName, string message);
}