using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
#elif ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.SpatialTracking;
#endif
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Shared;

namespace HurricaneVRExtensions.Simulator
{
    public class HVRHandsSimulator : MonoBehaviour
    {
        [Header("Required Components")]
        public GameObject Rig;
        public bool autoResolveDependencies = true;

        [Header("Settings")]
        public float handMovementSpeed = .1f;
        public float handRotationSpeed = 20f;
        public float slowDownRate = 0.05f;

        private float _currentMovementSpeed;
        private float _currentRotationSpeed;
        [SerializeField] private Vector3 _startHandsPositionOffset = new Vector3(0, 0.8f, 0.3f);

        [Header("Input")]
        #region Input
#if ENABLE_INPUT_SYSTEM
        public Key HandSlowdown = Key.LeftShift;

        public Key LeftHandKey = Key.Q;
        public Key RightHandKey = Key.E;
        public Key GripKey = Key.G;
        public Key PrimaryButtonKey = Key.Digit1;
        public Key SecondaryButtonKey = Key.Digit2;
        public Key JoystickButtonKey = Key.Digit3;

        public bool HandSlowdownPressed => Keyboard.current[HandSlowdown].isPressed;
        public bool UsingLeftHand => Keyboard.current[LeftHandKey].isPressed;
        public bool UsingRightHand => Keyboard.current[RightHandKey].isPressed;
        public bool RotatingHands => Mouse.current.middleButton.isPressed;
        public bool GripPressed => Keyboard.current[GripKey].isPressed;
        public bool TriggerPressed => Mouse.current.leftButton.isPressed;
        public bool PrimaryButtonStarted => Keyboard.current[PrimaryButtonKey].wasPressedThisFrame;
        public bool SecondaryButtonStarted => Keyboard.current[SecondaryButtonKey].wasPressedThisFrame;
        public bool JoystickButtonStarted => Keyboard.current[JoystickButtonKey].wasPressedThisFrame;
        public Vector2 MouseDelta => Mouse.current.delta.ReadValue();
        public Vector2 MouseDeltaScroll => Mouse.current.scroll.ReadValue();

#elif ENABLE_LEGACY_INPUT_MANAGER
		public KeyCode HandSlowdown = KeyCode.LeftShift;

        public KeyCode LeftHandKey = KeyCode.Q;
        public KeyCode RightHandKey = KeyCode.E;
        public KeyCode GripKey = KeyCode.G;
        public KeyCode PrimaryButtonKey = KeyCode.Alpha1;
        public KeyCode SecondaryButtonKey = KeyCode.Alpha2;
        public KeyCode JoystickButtonKey = KeyCode.Alpha3;

		public bool HandSlowdownPressed => Input.GetKey(HandSlowdown);
        public bool UsingLeftHand => Input.GetKey(LeftHandKey);
        public bool UsingRightHand => Input.GetKey(RightHandKey);
        public bool RotatingHands => Input.GetMouseButton(2);
        public bool GripPressed => Input.GetKey(GripKey);
        public bool TriggerPressed => Input.GetMouseButton(0);
        public bool PrimaryButtonStarted => Input.GetKeyDown(PrimaryButtonKey);
        public bool SecondaryButtonStarted => Input.GetKeyDown(SecondaryButtonKey);
        public bool JoystickButtonStarted => Input.GetKeyDown(JoystickButtonKey);
        public Vector2 MouseDelta => new Vector2(Input.GetAxis("Mouse X") * 10f, Input.GetAxis("Mouse Y") * 10f);
        public Vector2 MouseDeltaScroll => Input.mouseScrollDelta * 10f;
#endif
        #endregion

        [Header("Debug")]
        [SerializeField] private bool _showHandForwardAxis;
        [SerializeField] private bool _showTrackedControllerPosition;

        //Dependencies
        private Transform _controllerTargetLeft;
        private Transform _controllerTargetRight;
        private Camera _camera;

        [HideInInspector] public HVRHandGrabber HandGrabberLeft;
        [HideInInspector] public HVRHandGrabber HandGrabberRight;
        [HideInInspector] public HVRController ControllerLeft;
        [HideInInspector] public HVRController ControllerRight;

        private void Start()
        {
            if (!ResolveDependencies() || Application.isMobilePlatform)
            {
                enabled = false;
                return;
            }

            SetHandsInitialPosition();

            _currentMovementSpeed = handMovementSpeed;
        }

        private void Update()
        {
            HVRInputManager.Instance.LeftController.enabled = !UsingLeftHand;
            HVRInputManager.Instance.RightController.enabled = !UsingRightHand;

            HandleHandsInput();
            UpdateFingerCurls();
        }

        private void HandleHandsInput()
        {
            ResetButtonOnActivate(HVRHandSide.Left, HVRButtons.Grip, ref ControllerLeft.GripButtonState);
            ResetButtonOnActivate(HVRHandSide.Left, HVRButtons.Trigger, ref ControllerLeft.TriggerButtonState);

            ResetButtonOnActivate(HVRHandSide.Left, HVRButtons.Primary, ref ControllerLeft.PrimaryButtonState);
            ResetButtonOnActivate(HVRHandSide.Left, HVRButtons.Secondary, ref ControllerLeft.SecondaryButtonState);
            ResetButtonOnActivate(HVRHandSide.Left, HVRButtons.JoystickButton, ref ControllerLeft.JoystickButtonState);

            ResetButtonOnActivate(HVRHandSide.Right, HVRButtons.Grip, ref ControllerRight.GripButtonState);
            ResetButtonOnActivate(HVRHandSide.Right, HVRButtons.Trigger, ref ControllerRight.TriggerButtonState);

            ResetButtonOnActivate(HVRHandSide.Right, HVRButtons.Primary, ref ControllerRight.PrimaryButtonState);
            ResetButtonOnActivate(HVRHandSide.Right, HVRButtons.Secondary, ref ControllerRight.SecondaryButtonState);
            ResetButtonOnActivate(HVRHandSide.Right, HVRButtons.JoystickButton, ref ControllerRight.JoystickButtonState);

            if ((UsingLeftHand || UsingRightHand) && !RotatingHands)
                MoveHands();

            if (RotatingHands)
                RotateHands();

            if (UsingLeftHand)
            {
                SimulateButtonPress(HVRHandSide.Left, HVRButtons.Grip, ref ControllerLeft.GripButtonState, ref ControllerLeft.Grip, GripPressed);
                SimulateButtonPress(HVRHandSide.Left, HVRButtons.Trigger, ref ControllerLeft.TriggerButtonState, ref ControllerLeft.Trigger, TriggerPressed);

                SimulateButtonClick(HVRHandSide.Left, HVRButtons.Primary, ref ControllerLeft.PrimaryButtonState, ref ControllerLeft.PrimaryButton, PrimaryButtonStarted);
                SimulateButtonClick(HVRHandSide.Left, HVRButtons.Secondary, ref ControllerLeft.SecondaryButtonState, ref ControllerLeft.SecondaryButton, SecondaryButtonStarted);
                SimulateButtonClick(HVRHandSide.Left, HVRButtons.JoystickButton, ref ControllerLeft.JoystickButtonState, ref ControllerLeft.JoystickClicked, JoystickButtonStarted);

                HandleHandGrabbing(HandGrabberLeft, ControllerLeft);
            }

            if (UsingRightHand)
            {
                SimulateButtonPress(HVRHandSide.Right, HVRButtons.Grip, ref ControllerRight.GripButtonState, ref ControllerRight.Grip, GripPressed);
                SimulateButtonPress(HVRHandSide.Right, HVRButtons.Trigger, ref ControllerRight.TriggerButtonState, ref ControllerRight.Trigger, TriggerPressed);

                SimulateButtonClick(HVRHandSide.Right, HVRButtons.Primary, ref ControllerRight.PrimaryButtonState, ref ControllerRight.PrimaryButton, PrimaryButtonStarted);
                SimulateButtonClick(HVRHandSide.Right, HVRButtons.Secondary, ref ControllerRight.SecondaryButtonState, ref ControllerRight.SecondaryButton, SecondaryButtonStarted);
                SimulateButtonClick(HVRHandSide.Right, HVRButtons.JoystickButton, ref ControllerRight.JoystickButtonState, ref ControllerRight.JoystickClicked, JoystickButtonStarted);

                HandleHandGrabbing(HandGrabberRight, ControllerRight);
            }
        }

        #region Button Simulation
        private void SimulateButtonPress(HVRHandSide handSide, HVRButtons button, ref HVRButtonState buttonState, ref float buttonValue, bool isPressed)
        {
            float value = Convert.ToInt16(isPressed);

            if (value == buttonValue)
            {
                return;
            }

            buttonValue = value;
            buttonState.Value = buttonValue;
            SetButtonState(handSide, button, ref buttonState, isPressed);
        }

        private void SimulateButtonClick(HVRHandSide handSide, HVRButtons button, ref HVRButtonState buttonState, ref bool buttonValue, bool isPressed)
        {
            if (buttonValue == isPressed)
            {
                return;
            }

            buttonValue = isPressed;
            buttonState.Value = Convert.ToInt16(buttonValue);
            SetButtonState(handSide, button, ref buttonState, isPressed);
        }

        protected void SetButtonState(HVRHandSide handSide, HVRButtons button, ref HVRButtonState buttonState, bool pressed)
        {
            if (pressed)
            {
                if (!buttonState.Active)
                {
                    buttonState.JustActivated = true;
                    buttonState.Active = true;
                }
            }
            else
            {
                if (buttonState.Active)
                {
                    buttonState.Active = false;
                    buttonState.JustDeactivated = true;
                }
            }

            HVRController.SetButtonState(handSide, button, buttonState);
        }

        protected void ResetButtonSate(ref HVRButtonState buttonState)
        {
            buttonState.JustDeactivated = false;
            buttonState.JustActivated = false;
        }

        private void ResetButtonOnActivate(HVRHandSide handSide, HVRButtons button, ref HVRButtonState buttonState)
        {
            if (buttonState.JustActivated == false && buttonState.JustDeactivated == false)
                return;

            ResetButtonSate(ref buttonState);
            HVRController.SetButtonState(handSide, button, buttonState);
        }
        #endregion

        private void HandleHandGrabbing(HVRHandGrabber handGrabber, HVRController controller)
        {
            if (!handGrabber.AllowGrabbing)
            {
                handGrabber.AllowGrabbing = true;
            }

            // Check if grip button was just activated
            if (controller.GripButtonState.JustActivated)
            {
                if (handGrabber.IsGrabbing && !handGrabber.CanRelease)
                {
                    // Release and disable grabbing until the next frame
                    handGrabber.ForceRelease();
                    handGrabber.AllowGrabbing = false;
                }
            }

            // Check if grip button is active
            if (controller.GripButtonState.Active)
            {
                if (handGrabber.IsGrabbing && handGrabber.CanRelease)
                {
                    // Disable releasing
                    handGrabber.CanRelease = false;
                }
            }
        }

        private void MoveHands()
        {
            Vector3 direction = new Vector3(MouseDelta.x, MouseDelta.y, MouseDeltaScroll.y);

            _currentMovementSpeed = HandSlowdownPressed ? handMovementSpeed * slowDownRate : handMovementSpeed;

            if (UsingLeftHand)
                _controllerTargetLeft.position += _currentMovementSpeed * Time.deltaTime * (_camera.transform.rotation * direction);

            if (UsingRightHand)
                _controllerTargetRight.position += _currentMovementSpeed * Time.deltaTime * (_camera.transform.rotation * direction);
        }

        private void RotateHands()
        {
            Vector3 direction = new Vector3(-MouseDelta.y, MouseDelta.x, MouseDeltaScroll.y);

            _currentRotationSpeed = HandSlowdownPressed ? handRotationSpeed * slowDownRate : handRotationSpeed;

            if (UsingLeftHand)
                _controllerTargetLeft.Rotate(_currentRotationSpeed * Time.deltaTime * direction);

            if (UsingRightHand)
                _controllerTargetRight.Rotate(_currentRotationSpeed * Time.deltaTime * direction);
        }

        private void UpdateFingerCurls()
        {
            if (ControllerLeft.FingerSettings)
            {
                ControllerLeft.FingerSettings.Evaluate(
                    ControllerLeft.FingerCurls,
                    ControllerLeft.Grip,
                    ControllerLeft.Trigger,
                    ControllerLeft.TriggerTouchState.Active,
                    ControllerLeft.PrimaryTouchButtonState.Active,
                    ControllerLeft.SecondaryTouchButtonState.Active,
                    ControllerLeft.TrackPadTouchState.Active,
                    ControllerLeft.JoystickTouchState.Active,
                    ControllerLeft.Knuckles,
                    HVRInputManager.Instance.IsOpenXR);
            }

            if (ControllerRight.FingerSettings)
            {
                ControllerRight.FingerSettings.Evaluate(
                    ControllerRight.FingerCurls,
                    ControllerRight.Grip,
                    ControllerRight.Trigger,
                    ControllerRight.TriggerTouchState.Active,
                    ControllerRight.PrimaryTouchButtonState.Active,
                    ControllerRight.SecondaryTouchButtonState.Active,
                    ControllerRight.TrackPadTouchState.Active,
                    ControllerRight.JoystickTouchState.Active,
                    ControllerRight.Knuckles,
                    HVRInputManager.Instance.IsOpenXR);
            }

            for (int i = 0; i < 5; i++)
            {
                HVRController.LeftFingerCurls[i] = ControllerLeft.FingerCurls[i];
            }

            for (int i = 0; i < 5; i++)
            {
                HVRController.RightFingerCurls[i] = ControllerRight.FingerCurls[i];
            }
        }

        #region Initialization
        private void SetHandsInitialPosition()
        {
            Vector3 _leftControllerPosition = new Vector3(_controllerTargetLeft.transform.position.x - 0.1f, Rig.transform.position.y, Rig.transform.position.z);
            Vector3 _rightControllerPosition = new Vector3(_controllerTargetRight.transform.position.x + 0.1f, Rig.transform.position.y, Rig.transform.position.z);
            _controllerTargetLeft.transform.position = _leftControllerPosition + _startHandsPositionOffset;
            _controllerTargetRight.transform.position = _rightControllerPosition + _startHandsPositionOffset;
        }


        private bool ResolveDependencies()
        {
            if (!autoResolveDependencies)
                return true;

            HVRTrackedController[] trackedControllers = Rig.GetComponentsInChildren<HVRTrackedController>();

            if (trackedControllers.Length != 2)
            {
                Debug.Log(DependencyError("HVRTrackedController"));
                return false;
            }

            if (trackedControllers[0].GetComponent<TrackedPoseDriver>() != null)
            {
                TrackedPoseDriver[] trackedPoseDrivers = Rig.GetComponentsInChildren<TrackedPoseDriver>();
                foreach (TrackedPoseDriver driver in trackedPoseDrivers)
                {
                    driver.enabled = false;
                }
            }

            foreach (HVRTrackedController controller in trackedControllers)
            {
                if (controller.HandSide == HVRHandSide.Left)
                    _controllerTargetLeft = controller.gameObject.transform;
                else
                    _controllerTargetRight = controller.gameObject.transform;
            }

            if (Rig.GetComponentInChildren<HVRCamera>() != null)
                _camera = Rig.GetComponentInChildren<HVRCamera>().gameObject.GetComponent<Camera>();
            else
            {
                Debug.Log(DependencyError("Camera"));
                return false;
            }

            HVRHandGrabber[] handGrabbers = Rig.GetComponentsInChildren<HVRHandGrabber>();

            if (handGrabbers.Length != 2)
            {
                Debug.Log(DependencyError("HVRHandGrabber"));
                return false;
            }

            foreach (HVRHandGrabber grabber in handGrabbers)
            {
                if (grabber.HandSide == HVRHandSide.Left)
                    HandGrabberLeft = grabber;
                else
                    HandGrabberRight = grabber;
            }

            ControllerLeft = HVRInputManager.Instance.LeftController;
            ControllerRight = HVRInputManager.Instance.RightController;

            return true;
        }

        private string DependencyError(string component)
        {
            return string.Format("HVRHandsSimulator AutoResolveDependencies Error: No {0} component found. Make sure Rig parameter is assigned with Hurricane/Hexabody rig root object.", component);
        }
        #endregion

        #region Debug
        public void OnDrawGizmos()
        {
            if (_showHandForwardAxis && _controllerTargetLeft != null & _controllerTargetRight != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(_controllerTargetLeft.position, _controllerTargetLeft.position + _controllerTargetLeft.forward * 0.5f);
                Gizmos.DrawLine(_controllerTargetRight.position, _controllerTargetRight.position + _controllerTargetRight.forward * 0.5f);
            }

            if (_showTrackedControllerPosition && _controllerTargetLeft != null & _controllerTargetRight != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_controllerTargetLeft.position, 0.05f);
                Gizmos.DrawSphere(_controllerTargetRight.position, 0.05f);
            }
        }
        #endregion
    }
}
