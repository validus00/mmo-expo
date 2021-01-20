using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ChatManager class is for implementing Photon Chat and displaying messages in the chat panel
 */
public class ChatManager : MonoBehaviour, IChatClientListener {
    // For handling different 
    public enum ChannelType {
        hallChannel,
        boothChannel
    }

    // max number of channels
    private const int __maxMessages = 100;
    // Photon Chat client
    private ChatClient __chatClient;
    private string __username;
    // Showcase-wide channel
    private const string __announcementChannel = "Announcements";
    // Booth specific channel
    private string __boothChannel;
    // Hall specific channel
    private string __hallChannel;
    // For handling race condition if the user enters a channel prior to connecting to Photon Chat
    private bool __isConnected = false;

    [SerializeField]
    private GameObject __chatPanel, __textObject;
    
    [SerializeField]
    private InputField __chatBox;

    [SerializeField]
    private Color __playerMessage, __info;

    // messageList keeps tracks of recent messages
    [SerializeField]
    private List<Message> __messageList = new List<Message>();

    // Start is called before the first frame update
    void Start() {
        string passcodeMessage = string.Format("Passcode: {0}", PhotonNetwork.CurrentRoom.Name);
        __SendMessageToChat(passcodeMessage, Message.MessageType.info);
        __hallChannel = "Main Hall";
        __username = PhotonNetwork.NickName;

        // Create new Photon Chat client
        __chatClient = new ChatClient(this);
        __chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion,
            new AuthenticationValues(__username));
    }

    // Update is called once per frame
    void Update() {
        // Maintain service connection to Photon
        __chatClient.Service();

        // Enter key either sends a message or activates the chat input field
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (!string.IsNullOrEmpty(__chatBox.text)) {
                __chatClient.PublishMessage(__hallChannel, __chatBox.text);
                __chatBox.text = string.Empty;
            } else {
                __chatBox.ActivateInputField();
            }
        }
    }

    public void UpdateChannel(string channelName, ChatManager.ChannelType channelType) {
        switch (channelType) {
            case ChatManager.ChannelType.hallChannel:
                __hallChannel = channelName;
                break;
            case ChatManager.ChannelType.boothChannel:
                __boothChannel = channelName;
                break;
        }
    }

    // For leaving specific channels
    public void LeaveChannel(string channelName) {
        if (__isConnected) {
            __chatClient.Unsubscribe(new string[] { channelName });
        }
    }

    // For entering specific channels
    public void EnterChannel(string channelName) {
        if (__isConnected) {
            __chatClient.Subscribe(new string[] { channelName });
        }
    }

    // This method is for displaying received messages in chat panel
    private void __SendMessageToChat(string text, Message.MessageType messageType) {
        // Keep only 99 most recent messages before adding a new message to list
        if (__messageList.Count + 1 >= __maxMessages) {
            Destroy(__messageList[0].textObject.gameObject);
            __messageList.Remove(__messageList[0]);
        }

        // Create new Message object and add to list of messages
        Message message = new Message();

        GameObject newText = Instantiate(__textObject, __chatPanel.transform);
        message.textObject = newText.GetComponent<Text>();
        message.textObject.text = text;
        message.textObject.color = __MessageTypeColor(messageType);

        __messageList.Add(message);
    }

    // This method is for determining which color the message type is
    private Color __MessageTypeColor(Message.MessageType messageType) {
        // Default color
        Color color = __info;

        switch (messageType) {
            case Message.MessageType.playerMessage:
                color = __playerMessage;
                break;
        }

        return color;
    }

    #region IChatClientListener Callbacks
    public void DebugReturn(DebugLevel level, string message) {
    }

    public void OnDisconnected() {  
    }

    public void OnConnected() {
        __isConnected = true;
        Debug.Log("Connected to Photon Chat.");
        if (!string.IsNullOrWhiteSpace(__boothChannel)) {
            __chatClient.Subscribe(new string[] { __announcementChannel, __hallChannel, __boothChannel });
        } else {
            __chatClient.Subscribe(new string[] { __announcementChannel, __hallChannel });
        }
    }

    public void OnChatStateChange(ChatState state) {
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
            string message = string.Format("{0}: {1}", sender, messages[0]);
            __SendMessageToChat(message, Message.MessageType.playerMessage);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) {
    }

    public void OnSubscribed(string[] channels, bool[] results) {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++) {
            string subscriptionMessage = string.Format("You entered the {0} channel.", channels[i]);
            Debug.Log(subscriptionMessage);
            __SendMessageToChat(subscriptionMessage, Message.MessageType.info);
        }
    }

    public void OnUnsubscribed(string[] channels) {
        // Notify about connecting to new channels
        for (int i = 0; i < channels.Length; i++) {
            string unsubscriptionMessage = string.Format("You left the {0} channel.", channels[i]);
            Debug.Log(unsubscriptionMessage);
            __SendMessageToChat(unsubscriptionMessage, Message.MessageType.info);
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
    }

    public void OnUserSubscribed(string channel, string user) {
    }

    public void OnUserUnsubscribed(string channel, string user) {
    }
    #endregion
}

/*
 * Message class is for creating objects that hold text objects
 */
[System.Serializable]
public class Message {
    // For displaying text in chat in different colors depending on type of message 
    public enum MessageType {
        playerMessage,
        info
    }

    public Text textObject;
    public MessageType messageType;
}
