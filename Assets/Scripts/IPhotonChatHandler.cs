using System.Collections.Generic;

/*
 * This interface is for handling and abstracting Photon Chat functionality
 */
public interface IPhotonChatHandler
{
    // For connecting to chat service
    void ConnectToService();
    // For entering a specific channel
    void EnterChannel(string channelName);
    // Returns new messages and empty list of messages
    List<Message> GetNewMessages();
    // For leaving a specific channel
    void LeaveChannel(string channelName);
    // For initial setup
    void InitializeChannelNames(string[] channels);
    // For determining if connected to service
    bool IsConnected();
    // For determining if string is valid username
    bool IsValidUsername(string username);
    // For maintaining connection to service
    void MaintainService();
    // For sending message to a specific channel
    void SendChannelMessage(string channelName, string message);
    // For sending private messages to a specific user
    void SendPrivateMessage(string channelName, string message);
    // For getting user's entered name
    string UserEnteredName { get; }
    // For getting username
    string Username { get; }
}
