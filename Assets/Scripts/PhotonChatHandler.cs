using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

/*
 * PhotonChatHandler class implements IChatClientListener and IPhotonChatHandler interfaces and handles
 * Photon Chat functionalities
 */
public class PhotonChatHandler : IChatClientListener, IPhotonChatHandler {
    // For keeping track of new messages
    private List<Message> __newMessages = new List<Message>();
    // Current room name
    private readonly string __roomName;
    // Photon Chat client
    private readonly ChatClient __chatClient;
    // Subscribed channels
    private string[] __initialChannelNames;
    // To keep track if connected to Photon Chat
    private bool __isConnected;

    // Contructor that initializes Photon Chat client and connection
    public PhotonChatHandler() {
        __roomName = PhotonNetwork.CurrentRoom.Name;
        __AddNewMessage(string.Format("Passcode: {0}", __roomName), Message.MessageType.info);
        // Create new Photon Chat client
        __chatClient = new ChatClient(this);
        __isConnected = false;
    }

    public string Username {
        get { return PhotonNetwork.NickName; }
    }

    public string GetUsername(string name) {
        return name + PhotonNetwork.CurrentRoom.Name;
    }

    public void ConnectToService() {
        __chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion,
            new AuthenticationValues(PhotonNetwork.NickName));
    }

    public bool IsConnected() {
        return __isConnected;
    }

    public void MaintainService() {
        __chatClient.Service();
    }

    public void SendChannelMessage(string channelName, string message) {
        __chatClient.PublishMessage(__AppendRoomName(channelName), message);
    }

    public void InitializeChannelNames(string[] channels) {
        for (int i = 0; i < channels.Length; i++) {
            channels[i] = __AppendRoomName(channels[i]);
        }
        __initialChannelNames = channels;
    }

    public void LeaveChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName) && __isConnected) {
            __chatClient.Unsubscribe(new string[] { __AppendRoomName(channelName) });
        }
    }

    public void EnterChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName) && __isConnected) {
            __chatClient.Subscribe(new string[] { __AppendRoomName(channelName) });
        }
    }

    private string __AppendRoomName(string channelName) {
        return channelName + __roomName;
    }

    public static string RemoveRoomName(string text) {
        return text.Replace(PhotonNetwork.CurrentRoom.Name, string.Empty);
    }

    private void __AddNewMessage(string messageText, Message.MessageType messageType) {
        Message message = new Message();
        message.messageText = messageText;
        message.messageType = messageType;
        __newMessages.Add(message);
    }

    public List<Message> GetNewMessages() {
        List<Message> messages = __newMessages;
        __newMessages = new List<Message>();
        return messages;
    }

    public void SendPrivateMessage(string username, string message) {
        __chatClient.SendPrivateMessage(username, message);
    }

    #region IChatClientListener Callbacks
    public void DebugReturn(DebugLevel level, string message) {
        Debug.Log($"Photon Debug: {message}");
    }

    public void OnChatStateChange(ChatState state) {
        Debug.Log("OnChatStateChange is not implemented yet.");
    }

    public void OnConnected() {
        Debug.Log("Connected to Photon Chat.");
        __chatClient.Subscribe(__initialChannelNames);
        __isConnected = true;
    }

    public void OnDisconnected() {
        Debug.Log("OnDisconnected is not implemented yet.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        string username = PhotonNetwork.NickName;
        // Process all public messages received
        for (int i = 0; i < senders.Length; i++) {
            // Ensure that users with same username don't get each other's messages
            if (channelName.Contains(__roomName)) {
                string sender;
                if (senders[i] == username) {
                    sender = string.Format("{0} (You)", RemoveRoomName(username));
                } else {
                    sender = senders[0];
                }
                string message = string.Format("[{0}] {1}: {2}", RemoveRoomName(channelName), sender, messages[0]);
                __AddNewMessage(message, Message.MessageType.playerMessage);
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) {
        string username = PhotonNetwork.NickName;
        // Display the private message

        // Split channelName (ex: user1:user2) to extract sender/receiver information for formatting
        string[] users = channelName.Split(':');

        // Determine who is the recipient
        string recipient = users[0].Equals(username) ? users[1] : users[0];
        
        // Format the private message according to receiver/sender
        string privateMessage;
        if (sender == username) {
            privateMessage = string.Format("[Private] To {0}: {1}", RemoveRoomName(recipient), message.ToString());
        } else {
            privateMessage = string.Format("[Private] From {0}: {1}", RemoveRoomName(sender), message.ToString());
        }
        __AddNewMessage(privateMessage, Message.MessageType.privateMessage);
    }



    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        Debug.Log("OnStatusUpdate is not implemented yet.");
    }

    public void OnSubscribed(string[] channels, bool[] results) {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++) {
            // Ensure that users with same username don't get each other's messages
            if (channels[i].Contains(__roomName)) {
                string subscriptionMessage;
                if (results[i]) {
                    subscriptionMessage = string.Format("You entered the {0} channel.", RemoveRoomName(channels[i]));
                } else {
                    subscriptionMessage = string.Format("You failed to join the {0} channel.", RemoveRoomName(channels[i]));
                }
                Debug.Log(subscriptionMessage);
                __AddNewMessage(subscriptionMessage, Message.MessageType.info);
            }
        }
    }

    public void OnUnsubscribed(string[] channels) {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++) {
            // Ensure that users with same username don't get each other's messages
            if (channels[i].Contains(__roomName)) {
                string unsubscriptionMessage = string.Format("You left the {0} channel.", RemoveRoomName(channels[i]));
                Debug.Log(unsubscriptionMessage);
                __AddNewMessage(unsubscriptionMessage, Message.MessageType.info);
            }
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
