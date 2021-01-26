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
            photonChatHandler.GetNewMessages().Returns(new List<Message>());

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenMessagesIsAtNumberLimitThenGetLimitNumberOfMessages() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            List<Message> messages = new List<Message>();
            for (int i = 0; i < 200; i++) {
                Message newMessage = new Message();
                newMessage.messageText = "Hi there";
                newMessage.messageType = Message.MessageType.playerMessage;
                messages.Add(newMessage);
            }
            photonChatHandler.GetNewMessages().Returns(messages);
            photonChatHandler.IsConnected().Returns(true);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            yield return null;

            Assert.AreEqual(100, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenAChannelMessageIsSentThenGetMessage() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            List<Message> messages = new List<Message>();
            Message newMessage = new Message();
            newMessage.messageText = "Hi there";
            newMessage.messageType = Message.MessageType.playerMessage;
            messages.Add(newMessage);
            photonChatHandler.GetNewMessages().Returns(messages);
            photonChatHandler.IsConnected().Returns(true);

            __SetUpChatManager(chatManager, playerInputHandler, photonChatHandler, "Main Hall", "Hi there");

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).SendChannelMessage(Arg.Any<string>(), Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenChannelNameIsEmptyThenGetWarning() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            __SetUpChatManager(chatManager, playerInputHandler, photonChatHandler, null, "Hi there");

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
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, playerInputHandler, photonChatHandler, "Announcements", "Hi there");

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
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            __SetUpChatManager(chatManager, playerInputHandler, photonChatHandler, "Test", "Hi there");

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
        public IEnumerator WhenUpdateAnnouncementsChannelThenChannelIsNotUpdated() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel("General", ChatManager.ChannelType.announcementChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).InitializeChannels(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateBoothChannelAndPhotonChatIsConnectedThenInitializeChannelsIsNotCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel("MMO Expo", ChatManager.ChannelType.boothChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual("MMO Expo", chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).InitializeChannels(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateBoothChannelThenBoothChannelIsUpdated() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel("MMO Expo", ChatManager.ChannelType.boothChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual("MMO Expo", chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(2).InitializeChannels(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateHallChannelThenHallChannelIsUpdated() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel("Booths Hall North", ChatManager.ChannelType.hallChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Booths Hall North", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(2).InitializeChannels(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenLeaveChannelIsCalledThenHandlerLeaveChannelIsCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel("Test", ChatManager.ChannelType.boothChannel);
            chatManager.LeaveChannel("Test");

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual("Test", chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).LeaveChannel(Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenLeaveChannelIsCalledOnEmptyStringThenHandlerLeaveChannelIsNotCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.LeaveChannel(string.Empty);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.DidNotReceive().LeaveChannel(Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenEnterChannelIsCalledThenHandlerEnterChannelIsCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.EnterChannel("Test");

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).EnterChannel(Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenEnterChannelIsCalledOnEmptyStringThenHandlerEnterChannelIsNotCalled() {
            GameObject eventManager = new GameObject("ExpoEventManager");
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            __SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.EnterChannel(string.Empty);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual("Announcements", chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual("Main Hall", chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.DidNotReceive().EnterChannel(Arg.Any<string>());
        }

        private void __SetUpChatManager(ChatManager chatManager, IPlayerInputHandler playerInputHandler,
            IPhotonChatHandler photonChatHandler, string channelName, string message) {
            chatManager.playerInputHandler = playerInputHandler;
            chatManager.photonChatHandler = photonChatHandler;
            chatManager.playerMessageColor = new Color(255, 255, 255);
            chatManager.infoColor = new Color(255, 255, 0);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.chatPanel = chatPanel;

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<Text>();
            chatManager.textObject = textObject;

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.channelBox = channelBox;
            if (channelName != null) {
                channelBox.text = channelName;
            }

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.chatBox = chatBox;
            if (message != null) {
                chatBox.text = message;
            }
        }
    }
}
