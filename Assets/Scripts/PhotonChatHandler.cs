using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PhotonChatHandler : IChatClientListener, IPhotonChatHandler {
    // For keeping track of new messages
    private List<Message> __newMessages = new List<Message>();
    // Current room name
    private string __roomName;
    private string __username;
    // Photon Chat client
    private ChatClient __chatClient;
    // Subscribed channels
    private string[] __initialChannels;
    // To keep track if connected to Photon Chat
    private bool __isConnected = false;

    // For initial setup
    public void Initialize() {
        __roomName = PhotonNetwork.CurrentRoom.Name;
        __AddNewMessage(string.Format("Passcode: {0}", __roomName), Message.MessageType.info);
        __username = PhotonNetwork.NickName;
        // Create new Photon Chat client
        __chatClient = new ChatClient(this);
        __chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion,
            new AuthenticationValues(__username));
    }

    // For determining if connected to Photon Chat
    public bool IsConnected() {
        return __isConnected;
    }

    // For maintaining connection to Photon Chat
    public void MaintainService() {
        __chatClient.Service();
    }

    // For sending message to a specific channel
    public void SendChannelMessage(string channelName, string message) {
        __chatClient.PublishMessage(__AppendRoomName(channelName), message);
    }

    // For setting up the initial channels when first connecting to service
    public void InitializeChannels(string[] channels) {
        for (int i = 0; i < channels.Length; i++) {
            channels[i] = __AppendRoomName(channels[i]);
        }
        __initialChannels = channels;
    }

    // For leaving a specific channel
    public void LeaveChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName) && __isConnected) {
            __chatClient.Unsubscribe(new string[] { __AppendRoomName(channelName) });
        }
    }

    // For entering a specific channel
    public void EnterChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName) && __isConnected) {
            __chatClient.Subscribe(new string[] { __AppendRoomName(channelName) });
        }
    }

    private string __AppendRoomName(string channelName) {
        return channelName + __roomName;
    }

    private string __RemoveRoomName(string channelName) {
        return channelName.Replace(__roomName, string.Empty);
    }

    private void __AddNewMessage(string messageText, Message.MessageType messageType) {
        Message message = new Message();
        message.messageText = messageText;
        message.messageType = messageType;
        __newMessages.Add(message);
    }

    // Returns new messages and empty list of messages
    public List<Message> GetNewMessages() {
        List<Message> messages = __newMessages;
        __newMessages = new List<Message>();
        return messages;
    }

    #region IChatClientListener Callbacks
    public void DebugReturn(DebugLevel level, string message) {
        Debug.Log("DebugReturn is not implemented yet.");
    }

    public void OnChatStateChange(ChatState state) {
        Debug.Log("OnChatStateChange is not implemented yet.");
    }

    public void OnConnected() {
        __isConnected = true;
        Debug.Log("Connected to Photon Chat.");
        __chatClient.Subscribe(__initialChannels);
    }

    public void OnDisconnected() {
        Debug.Log("OnDisconnected is not implemented yet.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        // Process all public messages recevied
        for (int i = 0; i < senders.Length; i++) {
            string sender;
            if (senders[i] == __username) {
                sender = string.Format("{0} (You)", __username);
            } else {
                sender = senders[0];
            }
            string message = string.Format("[{0}] {1}: {2}", __RemoveRoomName(channelName), sender, messages[0]);
            __AddNewMessage(message, Message.MessageType.playerMessage);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) {
        Debug.Log("OnPrivateMessage is not implemented yet.");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        Debug.Log("OnStatusUpdate is not implemented yet.");
    }

    public void OnSubscribed(string[] channels, bool[] results) {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++) {
            string subscriptionMessage;
            if (results[i]) {
                subscriptionMessage = string.Format("You entered the {0} channel.", __RemoveRoomName(channels[i]));
            } else {
                subscriptionMessage = string.Format("You failed to join the {0} channel.", __RemoveRoomName(channels[i]));
            }
            Debug.Log(subscriptionMessage);
            __AddNewMessage(subscriptionMessage, Message.MessageType.info);
        }
    }

    public void OnUnsubscribed(string[] channels) {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++) {
            string unsubscriptionMessage = string.Format("You left the {0} channel.", __RemoveRoomName(channels[i]));
            Debug.Log(unsubscriptionMessage);
            __AddNewMessage(unsubscriptionMessage, Message.MessageType.info);
        }
    }

    public void OnUserSubscribed(string channel, string user) {
        Debug.Log("OnUserSubscribed is not implemented yet.");
    }

    public void OnUserUnsubscribed(string channel, string user) {
        Debug.Log("OnUserUnsubscribed is not implemented yet.");
    }
    #endregion
}
