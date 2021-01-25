using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests {
    public class ChatManagerTests {
        [UnityTest]
        public IEnumerator WhenOnStartThenGetNoMessages() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenAChannelMessageIsSentThenGetMessage() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            List<Message> messages = new List<Message>();
            Message newMessage = new Message();
            newMessage.messageText = "Hi there";
            newMessage.messageType = Message.MessageType.playerMessage;
            messages.Add(newMessage);
            photonChatHandler.GetNewMessages().Returns(messages);
            photonChatHandler.IsConnected().Returns(true);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            channelBox.text = "Main Hall";
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatBox.text = "Hi there";
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenMessagesIsAtNumberLimitThenGetLimitNumberOfMessages() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            List<Message> messages = new List<Message>();
            for (int i = 0; i < 200; i++) {
                Message newMessage = new Message();
                newMessage.messageText = "Hi there";
                newMessage.messageType = Message.MessageType.playerMessage;
                messages.Add(newMessage);
            }
            photonChatHandler.GetNewMessages().Returns(messages);
            photonChatHandler.IsConnected().Returns(true);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            channelBox.text = "Main Hall";
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatBox.text = "Hi there";
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            yield return null;

            Assert.AreEqual(100, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenChannelNameIsEmptyThenGetWarning() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            channelBox.text = string.Empty;
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatBox.text = "Hi there";
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("No channel or username specified.", chatManager.GetMessages()[0].messageText);
            Assert.AreEqual(Message.MessageType.info, chatManager.GetMessages()[0].messageType);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenChatNotConnectedYetThenGetWarning() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            channelBox.text = "Announcements";
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatBox.text = "Hi there";
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("Not connected to chat yet.", chatManager.GetMessages()[0].messageText);
            Assert.AreEqual(Message.MessageType.info, chatManager.GetMessages()[0].messageType);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenChannelNameIsWrongThenGetWarning() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            channelBox.text = "Test";
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatBox.text = "Hi there";
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("You are not in \"Test\" channel. Cannot send message.",
                chatManager.GetMessages()[0].messageText);
            Assert.AreEqual(Message.MessageType.info, chatManager.GetMessages()[0].messageType);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenUpdateBoothChannelThenBoothChannelIsUpdated() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            chatManager.UpdateChannel("MMO Expo", ChatManager.ChannelType.boothChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual("MMO Expo", chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received().InitializeChannels(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateHallChannelThenHallChannelIsUpdated() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            chatManager.UpdateChannel("Booths Hall North", ChatManager.ChannelType.hallChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Booths Hall North", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received().InitializeChannels(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenLeaveChannelIsCalledThenHandlerLeaveChannelIsCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            chatManager.UpdateChannel("Test", ChatManager.ChannelType.boothChannel);
            chatManager.LeaveChannel("Test");

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            photonChatHandler.Received().LeaveChannel(Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenEnterChannelIsCalledThenHandlerEnterChannelIsCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            chatManager.InitializePlayerInputHandler(playerInputHandler);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            chatManager.InitializePhotonChatHandler(photonChatHandler);
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.InitializeChatPanel(chatPanel);

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.InitializeTextObject(textObject);

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.InitializeChannelBox(channelBox);

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.InitializeChatBox(chatBox);

            chatManager.InitializePlayerMessageColor(new Color(255, 255, 255));
            chatManager.InitializeInfoColor(new Color(255, 255, 0));

            chatManager.EnterChannel("Test");

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            photonChatHandler.Received().EnterChannel(Arg.Any<string>());
        }
    }
}
