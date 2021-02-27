using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/*
 * PhotonChatHandler class implements IChatClientListener and IPhotonChatHandler interfaces and handles
 * Photon Chat functionalities
 */
public class PhotonChatHandler : IChatClientListener, IPhotonChatHandler
{
    // For keeping track of new messages
    private List<Message> _newMessages = new List<Message>();
    // Current room name
    private readonly string _roomName;
    // Photon Chat client
    private readonly ChatClient _chatClient;
    // Subscribed channels
    private string[] _initialChannelNames;
    // To keep track if connected to Photon Chat
    private bool _isConnected;

    // Constructor that initializes Photon Chat client and connection
    public PhotonChatHandler()
    {
        _roomName = PhotonNetwork.CurrentRoom.Name;
        AddNewMessage(string.Format("Passcode: {0}", _roomName), Message.MessageType.info);
        // Create new Photon Chat client
        _chatClient = new ChatClient(this);
        _isConnected = false;
    }

    public bool IsValidUsername(string username)
    {
        string networkUsername = username + PhotonNetwork.CurrentRoom.Name;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (networkUsername.Equals(player.NickName))
            {
                return true;
            }
        }

        return false;
    }

    public string UserEnteredName
    {
        get { return RemoveRoomName(Username); }
    }

    public string Username
    {
        get { return PhotonNetwork.NickName; }
    }

    public string GetUsername(string name)
    {
        return name + PhotonNetwork.CurrentRoom.Name;
    }

    public void ConnectToService()
    {
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion,
            new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }

    public bool IsConnected()
    {
        return _isConnected;
    }

    public void MaintainService()
    {
        _chatClient.Service();
    }

    public void SendChannelMessage(string channelName, string message)
    {
        _chatClient.PublishMessage(AppendRoomName(channelName), message);
    }

    public void InitializeChannelNames(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            channels[i] = AppendRoomName(channels[i]);
        }
        _initialChannelNames = channels;
    }

    public void LeaveChannel(string channelName)
    {
        if (!string.IsNullOrWhiteSpace(channelName) && _isConnected)
        {
            _chatClient.Unsubscribe(new string[] { AppendRoomName(channelName) });
        }
    }

    public void EnterChannel(string channelName)
    {
        if (!string.IsNullOrWhiteSpace(channelName) && _isConnected)
        {
            _chatClient.Subscribe(new string[] { AppendRoomName(channelName) });
        }
    }

    private string AppendRoomName(string channelName)
    {
        return channelName + _roomName;
    }

    public static string RemoveRoomName(string text)
    {
        return text.Replace(PhotonNetwork.CurrentRoom.Name, string.Empty);
    }

    private void AddNewMessage(string messageText, Message.MessageType messageType)
    {
        Message message = new Message();
        message.MessageText = messageText;
        message.MsgType = messageType;
        _newMessages.Add(message);
    }

    public List<Message> GetNewMessages()
    {
        List<Message> messages = _newMessages;
        _newMessages = new List<Message>();
        return messages;
    }

    public void SendPrivateMessage(string username, string message)
    {
        _chatClient.SendPrivateMessage(username + PhotonNetwork.CurrentRoom.Name, message);
    }

    #region IChatClientListener Callbacks
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Photon Debug: {message}");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange is not implemented yet.");
    }

    public void OnConnected()
    {
        Debug.Log("Connected to Photon Chat.");
        _chatClient.Subscribe(_initialChannelNames);
        _isConnected = true;
    }

    public void OnDisconnected()
    {
        Debug.Log("OnDisconnected is not implemented yet.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string username = PhotonNetwork.NickName;
        // Process all public messages received
        for (int i = 0; i < senders.Length; i++)
        {
            // Ensure that users with same username don't get each other's messages
            if (channelName.Contains(_roomName))
            {
                string sender;
                if (senders[i] == username)
                {
                    sender = string.Format("{0} (You)", RemoveRoomName(username));
                }
                else
                {
                    sender = senders[0];
                }
                string message = string.Format("[<link=\"{0}\">{0}</link>] {1}: {2}", RemoveRoomName(channelName), sender, messages[0]);
                AddNewMessage(message, Message.MessageType.playerMessage);
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string username = PhotonNetwork.NickName;
        // Display the private message

        // Split channelName (ex: user1:user2) to extract sender/receiver information for formatting
        string[] users = channelName.Split(':');

        // Determine who is the recipient
        string recipient = users[0].Equals(username) ? users[1] : users[0];

        // Format the private message according to receiver/sender
        string privateMessage;
        if (sender == username)
        {
            privateMessage = string.Format("[Private] To <link=\"{0}\"><u>{0}</u></link>: {1}", RemoveRoomName(recipient), message.ToString());
        }
        else
        {
            privateMessage = string.Format("[Private] From <link=\"{0}\"><u>{0}</u></link>: {1}", RemoveRoomName(sender), message.ToString());
        }
        AddNewMessage(privateMessage, Message.MessageType.privateMessage);
    }



    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("OnStatusUpdate is not implemented yet.");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++)
        {
            // Ensure that users with same username don't get each other's messages
            if (channels[i].Contains(_roomName))
            {
                string subscriptionMessage;
                if (results[i])
                {
                    subscriptionMessage = string.Format("You entered the <link=\"{0}\"><u>{0}</u></link> channel.", RemoveRoomName(channels[i]));
                }
                else
                {
                    subscriptionMessage = string.Format("You failed to join the {0} channel.", RemoveRoomName(channels[i]));
                }
                Debug.Log(subscriptionMessage);
                AddNewMessage(subscriptionMessage, Message.MessageType.info);
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++)
        {
            // Ensure that users with same username don't get each other's messages
            if (channels[i].Contains(_roomName))
            {
                string unsubscriptionMessage = string.Format("You left the {0} channel.", RemoveRoomName(channels[i]));
                Debug.Log(unsubscriptionMessage);
                AddNewMessage(unsubscriptionMessage, Message.MessageType.info);
            }
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log("OnUserSubscribed is not implemented yet.");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log("OnUserUnsubscribed is not implemented yet.");
    }
    #endregion
}
