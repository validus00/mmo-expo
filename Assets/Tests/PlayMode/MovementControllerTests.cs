using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests {
    public class MovementControllerTests {
        [UnityTest]
        public IEnumerator WhenNoUserInputsThenPositionWillNotChange() {
            GameObject user = new GameObject("Player");

            __SetUpMovementController(user, null);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenUserInputsForwardMovementThenPositionChanges() {
            GameObject user = new GameObject("Player");

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));

            __SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(user.transform.position.z > 0);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(1f, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenUserInputsLeftMovementThenPositionChanges() {
            GameObject user = new GameObject("Player");

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(-1, 0, 0));

            __SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(0 > user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(-1f, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenChatInputFieldIsInFocusThenNotInFocusThenPositionWillChange() {
            GameObject user = new GameObject("Player");

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));
            IInputFieldHandler channelBoxHandler = Substitute.For<IInputFieldHandler>();
            IInputFieldHandler chatBoxHandler = Substitute.For<IInputFieldHandler>();
            channelBoxHandler.isFocused().Returns(false);
            chatBoxHandler.isFocused().Returns(true);

            __SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();
            MovementController movementController = user.GetComponent<MovementController>();
            movementController.channelBoxHandler = channelBoxHandler;
            movementController.chatBoxHandler = chatBoxHandler;

            yield return null;

            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));

            channelBoxHandler.isFocused().Returns(false);
            chatBoxHandler.isFocused().Returns(false);

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(user.transform.position.z > 0);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(1f, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenRightMouseButtonIsHeldDownThenCameraMoves() {
            GameObject user = new GameObject("Player");

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetRightClickInputHeld().Returns(true);
            playerInputHandler.GetLookInputsVertical().Returns(1f);
            playerInputHandler.GetLookInputsHorizontal().Returns(1f);

            Quaternion originalRotation = user.transform.rotation;

            __SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsFalse(Cursor.visible);
            Assert.AreEqual(CursorLockMode.Locked, Cursor.lockState);
            Assert.AreNotEqual(originalRotation, user.transform.rotation);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        [UnityTest]
        public IEnumerator WhenRightMouseButtonIsNotHeldDownThenCameraDoesNotMove() {
            GameObject user = new GameObject("Player");

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetRightClickInputHeld().Returns(false);
            playerInputHandler.GetLookInputsVertical().Returns(1f);
            playerInputHandler.GetLookInputsHorizontal().Returns(1f);

            Quaternion originalRotation = user.transform.rotation;

            __SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(Cursor.visible);
            Assert.AreEqual(CursorLockMode.None, Cursor.lockState);
            Assert.AreEqual(originalRotation, user.transform.rotation);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat("Horizontal"));
            Assert.AreEqual(0, animator.GetFloat("Vertical"));
        }

        private void __SetUpMovementController(GameObject user, IPlayerInputHandler playerInputHandler) {
            GameObject channelBoxObject = new GameObject("ChannelInputField");
            channelBoxObject.AddComponent<InputField>();
            channelBoxObject.AddComponent<InputFieldHandler>();

            GameObject chatBoxObject = new GameObject("MessageInputField");
            chatBoxObject.AddComponent<InputField>();
            chatBoxObject.AddComponent<InputFieldHandler>();

            user.AddComponent<CharacterController>();
            user.AddComponent<Animator>().runtimeAnimatorController
                = Resources.Load("GenericAnimationController") as RuntimeAnimatorController;

            GameObject camera = new GameObject("Camera");
            MovementController movementController = user.AddComponent<MovementController>();
            movementController.playerInputHandler = playerInputHandler;
            movementController.fpsCamera = camera;
            user.transform.position = Vector3.zero;
        }
    }
}
