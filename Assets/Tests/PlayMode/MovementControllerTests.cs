using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests {
    public class MovementControllerTests {
        [UnityTest]
        public IEnumerator WhenUserInputsForwardMovementThenPositionChanges() {
            __AddInputFields();

            GameObject user = new GameObject("Player");
            user.AddComponent<CharacterController>();
            Animator animator = user.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("GenericAnimationController") as RuntimeAnimatorController;

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));
            MovementController movementController = user.AddComponent<MovementController>();
            movementController.playerInputHandler = playerInputHandler;
            user.transform.position = Vector3.zero;

            yield return null;

            Assert.IsTrue(user.transform.position.z > 0);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(1f, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenChatInputFieldIsInFocusThenPositionWillNotChange() {
            GameObject user = new GameObject("Player");
            user.AddComponent<CharacterController>();
            Animator animator = user.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("GenericAnimationController") as RuntimeAnimatorController;

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));
            IInputFieldHandler channelBoxHandler = Substitute.For<IInputFieldHandler>();
            IInputFieldHandler chatBoxHandler = Substitute.For<IInputFieldHandler>();
            channelBoxHandler.isFocused().Returns(false);
            chatBoxHandler.isFocused().Returns(true);

            MovementController movementController = user.AddComponent<MovementController>();
            movementController.playerInputHandler = playerInputHandler;
            movementController.channelBoxHandler = channelBoxHandler;
            movementController.chatBoxHandler = chatBoxHandler;
            user.transform.position = Vector3.zero;

            yield return null;

            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenRightMouseButtonIsHeldDownThenCameraMoves() {
            __AddInputFields();

            GameObject user = new GameObject("Player");
            user.AddComponent<CharacterController>();
            Animator animator = user.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("GenericAnimationController") as RuntimeAnimatorController;

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetRightClickInputHeld().Returns(true);
            playerInputHandler.GetLookInputsVertical().Returns(1f);
            playerInputHandler.GetLookInputsHorizontal().Returns(1f);

            Quaternion originalRotation = user.transform.rotation;

            MovementController movementController = user.AddComponent<MovementController>();
            movementController.playerInputHandler = playerInputHandler;
            GameObject camera = new GameObject("Camera");
            movementController.fpsCamera = camera;
            user.transform.position = Vector3.zero;

            yield return null;

            Assert.AreNotEqual(originalRotation, user.transform.rotation);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenRightMouseButtonIsNotHeldDownThenCameraDoesNotMove() {
            __AddInputFields();

            GameObject user = new GameObject("Player");
            user.AddComponent<CharacterController>();
            Animator animator = user.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load("GenericAnimationController") as RuntimeAnimatorController;

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetRightClickInputHeld().Returns(false);
            playerInputHandler.GetLookInputsVertical().Returns(1f);
            playerInputHandler.GetLookInputsHorizontal().Returns(1f);

            Quaternion originalRotation = user.transform.rotation;

            MovementController movementController = user.AddComponent<MovementController>();
            movementController.playerInputHandler = playerInputHandler;
            GameObject camera = new GameObject("Camera");
            movementController.fpsCamera = camera;
            user.transform.position = Vector3.zero;

            yield return null;

            Assert.AreEqual(originalRotation, user.transform.rotation);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        private void __AddInputFields() {
            GameObject channelBoxObject = new GameObject("ChannelInputField");
            channelBoxObject.AddComponent<InputField>();
            channelBoxObject.AddComponent<InputFieldHandler>();
            GameObject chatBoxObject = new GameObject("MessageInputField");
            chatBoxObject.AddComponent<InputField>();
            chatBoxObject.AddComponent<InputFieldHandler>();
        }
    }
}
