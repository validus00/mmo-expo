using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class MovementControllerTests
    {
        private const string K_Player = "Player";

        [UnityTest]
        public IEnumerator WhenNoUserInputsThenPositionWillNotChange()
        {
            GameObject user = new GameObject(K_Player);

            SetUpMovementController(user, null);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenUserInputsForwardMovementThenPositionChanges()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(user.transform.position.z > 0);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(1f, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenUserInputsLeftMovementThenPositionChanges()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(-1, 0, 0));

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(0 > user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(-1f, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenChatInputFieldIsInFocusThenNotInFocusThenPositionWillChange()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));
            IInputFieldHandler channelBoxHandler = Substitute.For<IInputFieldHandler>();
            IInputFieldHandler chatBoxHandler = Substitute.For<IInputFieldHandler>();
            channelBoxHandler.isFocused().Returns(false);
            chatBoxHandler.isFocused().Returns(true);

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();
            MovementController movementController = user.GetComponent<MovementController>();
            movementController.ChannelBoxHandler = channelBoxHandler;
            movementController.ChatBoxHandler = chatBoxHandler;

            yield return null;

            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));

            channelBoxHandler.isFocused().Returns(false);
            chatBoxHandler.isFocused().Returns(false);

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(user.transform.position.z > 0);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(1f, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenRightMouseButtonIsHeldDownThenCameraMoves()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetRightClickInputHeld().Returns(true);
            playerInputHandler.GetLookInputsVertical().Returns(1f);
            playerInputHandler.GetLookInputsHorizontal().Returns(1f);

            Quaternion originalRotation = user.transform.rotation;

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsFalse(Cursor.visible);
            //            Assert.AreEqual(CursorLockMode.Locked, Cursor.lockState);
            Assert.AreNotEqual(originalRotation, user.transform.rotation);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenRightMouseButtonIsNotHeldDownThenCameraDoesNotMove()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetRightClickInputHeld().Returns(false);
            playerInputHandler.GetLookInputsVertical().Returns(1f);
            playerInputHandler.GetLookInputsHorizontal().Returns(1f);

            Quaternion originalRotation = user.transform.rotation;

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(Cursor.visible);
            Assert.AreEqual(CursorLockMode.None, Cursor.lockState);
            Assert.AreEqual(originalRotation, user.transform.rotation);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenAPanelIsActiveThenPositionWillNotChange()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            MovementController movementController = user.GetComponent<MovementController>();
            movementController.PanelManager.IsAnyPanelActive().Returns(true);

            yield return null;

            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));
        }

        [UnityTest]
        public IEnumerator WhenTabIsPressedThenCursorIsNormalAndToggleExitEventPanelIsCalled()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetTabKey().Returns(true);

            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            yield return null;

            Assert.IsTrue(Cursor.visible);
            Assert.AreEqual(CursorLockMode.None, Cursor.lockState);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, user.transform.position.z);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Vertical));
            playerInputHandler.Received(1).GetTabKey();
        }

        [UnityTest]
        public IEnumerator WhenCharacterControllerIsDiabledThenAnyMovementWillEnableIt()
        {
            GameObject user = new GameObject(K_Player);

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();


            SetUpMovementController(user, playerInputHandler);
            Animator animator = user.GetComponent<Animator>();

            CharacterController characterController = user.GetComponent<CharacterController>();
            characterController.enabled = false;

            yield return null;

            Assert.IsFalse(characterController.enabled);

            playerInputHandler.GetMoveInput().Returns(new Vector3(0, 0, 1));

            yield return null;

            Assert.IsTrue(characterController.enabled);
            Assert.IsTrue(user.transform.position.z > 0);
            Assert.AreEqual(0, user.transform.position.x);
            Assert.AreEqual(0, user.transform.position.y);
            Assert.AreEqual(0, animator.GetFloat(GameConstants.k_Horizontal));
            Assert.AreEqual(1f, animator.GetFloat(GameConstants.k_Vertical));
        }

        private void SetUpMovementController(GameObject user, IPlayerInputHandler playerInputHandler)
        {
            GameObject channelBoxObject = new GameObject(GameConstants.k_ChannelInputField);
            channelBoxObject.AddComponent<InputField>();
            channelBoxObject.AddComponent<InputFieldHandler>();

            GameObject chatBoxObject = new GameObject(GameConstants.k_MessageInputField);
            chatBoxObject.AddComponent<InputField>();
            chatBoxObject.AddComponent<InputFieldHandler>();

            user.AddComponent<CharacterController>();
            user.AddComponent<Animator>().runtimeAnimatorController
                = Resources.Load(GameConstants.k_AnimationController) as RuntimeAnimatorController;

            GameObject camera = new GameObject(GameConstants.k_Camera);
            MovementController movementController = user.AddComponent<MovementController>();
            movementController.PlayerInputHandler = playerInputHandler;
            movementController.FpsCamera = camera;
            user.transform.position = Vector3.zero;

            movementController.PanelManager = Substitute.For<IPanelManager>();
        }
    }
}
