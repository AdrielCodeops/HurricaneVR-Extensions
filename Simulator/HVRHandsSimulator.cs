using UnityEngine;
using UnityEngine.XR;
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

        [Header("Parameters")]
        [SerializeField] private float _handMovementSpeed = .1f;
        [SerializeField] private float _handsRotationSpeed = 20f;
        [SerializeField] private Vector3 _startHandsPositionOffset = new Vector3(0, 0.8f, 0.3f);

        #region Input
#if ENABLE_INPUT_SYSTEM
        public Key LeftHandKey = Key.Q;
        public Key RightHandKey = Key.E;
        public Key GripKey = Key.G;
        public Key PrimaryButtonKey = Key.Digit1;
        public Key SecondaryButtonKey = Key.Digit2;
        public Key JoystickButtonKey = Key.Digit3;

        public bool UsingLeftHand => Keyboard.current[LeftHandKey].isPressed;
        public bool UsingRightHand => Keyboard.current[RightHandKey].isPressed;
        public bool RotatingHands => Mouse.current.middleButton.isPressed;
        public bool GripStarted => Keyboard.current[GripKey].wasPressedThisFrame;
        public bool GripEnded => Keyboard.current[GripKey].wasReleasedThisFrame;
        public bool TriggerStarted => Mouse.current.leftButton.wasPressedThisFrame;
        public bool TriggerEnded => Mouse.current.leftButton.wasReleasedThisFrame;
        public bool PrimaryButtonStarted => Keyboard.current[PrimaryButtonKey].wasPressedThisFrame;
        public bool PrimaryButtonEnded => Keyboard.current[PrimaryButtonKey].wasReleasedThisFrame;
        public bool SecondaryButtonStarted => Keyboard.current[SecondaryButtonKey].wasPressedThisFrame;
        public bool SecondaryButtonEnded => Keyboard.current[SecondaryButtonKey].wasReleasedThisFrame;
        public bool JoystickButtonStarted => Keyboard.current[JoystickButtonKey].wasPressedThisFrame;
        public bool JoystickButtonEnded => Keyboard.current[JoystickButtonKey].wasReleasedThisFrame;
        public Vector2 MouseDelta => Mouse.current.delta.ReadValue();
        public Vector2 MouseDeltaScroll => Mouse.current.scroll.ReadValue();

#elif ENABLE_LEGACY_INPUT_MANAGER
        public KeyCode LeftHandKey = KeyCode.Q;
        public KeyCode RightHandKey = KeyCode.E;
        public KeyCode GripKey = KeyCode.G;
        public KeyCode PrimaryButtonKey = KeyCode.Alpha1;
        public KeyCode SecondaryButtonKey = KeyCode.Alpha2;
        public KeyCode JoystickButtonKey = KeyCode.Alpha3;

        public bool UsingLeftHand => Input.GetKey(LeftHandKey);
        public bool UsingRightHand => Input.GetKey(RightHandKey);
        public bool RotatingHands => Input.GetMouseButton(2);
        public bool GripStarted => Input.GetKeyDown(GripKey);
        public bool GripEnded => Input.GetKeyUp(GripKey);
        public bool TriggerStarted => Input.GetMouseButtonDown(0);
        public bool TriggerEnded => Input.GetMouseButtonUp(0);
        public bool PrimaryButtonStarted => Input.GetKeyDown(PrimaryButtonKey);
        public bool PrimaryButtonEnded => Input.GetKeyUp(PrimaryButtonKey);
        public bool SecondaryButtonStarted => Input.GetKeyDown(SecondaryButtonKey);
        public bool SecondaryButtonEnded => Input.GetKeyUp(SecondaryButtonKey);
        public bool JoystickButtonStarted => Input.GetKeyDown(JoystickButtonKey);
        public bool JoystickButtonEnded => Input.GetKeyUp(JoystickButtonKey);
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

        private bool _lockLeftHandGrabbable;
        private bool _lockRightHandGrabbable;

        private void Start()
        {
            if (!ResolveDependencies() || Application.isMobilePlatform)
            {
                enabled = false;
                return;
            }

            SetHandsInitialPosition();
        }

        private void Update()
        {
            HVRInputManager.Instance.LeftController.enabled = !UsingLeftHand;
            HVRInputManager.Instance.RightController.enabled = !UsingRightHand;

            HandGrabberLeft.CanRelease = !_lockLeftHandGrabbable;
            HandGrabberRight.CanRelease = !_lockRightHandGrabbable;

            ProcessHandsInput();

            UpdateFingerCurls();
        }

        private void ProcessHandsInput()
        {
            if ((UsingLeftHand || UsingRightHand) && !RotatingHands)
                MoveHands();

            if (RotatingHands)
                RotateHands();

            if (UsingLeftHand)
            {
                if (HandGrabberLeft.IsGrabbing && _lockLeftHandGrabbable == false)
                {
                    _lockLeftHandGrabbable = true;
                }

                if (GripStarted)
                    SimulateGrab(ControllerLeft);
                if (TriggerStarted)
                    SimulateTriggerButton(ControllerLeft, true);
                if (PrimaryButtonStarted)
                    SimulatePrimaryButton(ControllerLeft, true);
                if (SecondaryButtonStarted)
                    SimulateSecondaryButton(ControllerLeft, true);
                if (JoystickButtonStarted)
                    SimulateJoystickButton(ControllerLeft, true);
            }

            if (UsingRightHand)
            {
                if (HandGrabberRight.IsGrabbing && _lockRightHandGrabbable == false)
                {
                    _lockRightHandGrabbable = true;
                }

                if (GripStarted)
                    SimulateGrab(ControllerRight);
                if (TriggerStarted)
                    SimulateTriggerButton(ControllerRight, true);
                if (PrimaryButtonStarted)
                    SimulatePrimaryButton(ControllerRight, true);
                if (SecondaryButtonStarted)
                    SimulateSecondaryButton(ControllerRight, true);
                if (JoystickButtonStarted)
                    SimulateJoystickButton(ControllerRight, true);
            }

            if (GripEnded)
            {
                SimulateGripButton(ControllerLeft, false);
                SimulateGripButton(ControllerRight, false);
            }

            if (TriggerEnded)
            {
                SimulateTriggerButton(ControllerLeft, false);
                SimulateTriggerButton(ControllerRight, false);
            }

            if (PrimaryButtonEnded)
            {
                SimulatePrimaryButton(ControllerLeft, false);
                SimulatePrimaryButton(ControllerRight, false);
            }

            if (SecondaryButtonEnded)
            {
                SimulateSecondaryButton(ControllerLeft, false);
                SimulateSecondaryButton(ControllerRight, false);
            }

            if (JoystickButtonEnded)
            {
                SimulateJoystickButton(ControllerLeft, false);
                SimulateJoystickButton(ControllerRight, false);
            }
        }

        public void SimulateTriggerButton(HVRController controller, bool down)
        {
            if (down)
                SimulateButtonDown(ref controller.TriggerButtonState, ref controller.Trigger);
            else
                SimulateButtonUp(ref controller.TriggerButtonState, ref controller.Trigger);
        }

        public void SimulateGripButton(HVRController controller, bool down)
        {
            if (down)
                SimulateButtonDown(ref controller.GripButtonState, ref controller.Grip);
            else
                SimulateButtonUp(ref controller.GripButtonState, ref controller.Grip);
        }

        public void SimulatePrimaryButton(HVRController controller, bool down)
        {
            if (down)
                SimulateButtonDown(ref controller.PrimaryButtonState, ref controller.PrimaryButton);
            else
                SimulateButtonUp(ref controller.PrimaryButtonState, ref controller.PrimaryButton);
        }

        public void SimulateSecondaryButton(HVRController controller, bool down)
        {
            if (down)
                SimulateButtonDown(ref controller.SecondaryButtonState, ref controller.SecondaryButton);
            else
                SimulateButtonUp(ref controller.SecondaryButtonState, ref controller.SecondaryButton);
        }

        public void SimulateJoystickButton(HVRController controller, bool down)
        {
            if (down)
                SimulateButtonDown(ref controller.JoystickButtonState, ref controller.JoystickClicked);
            else
                SimulateButtonUp(ref controller.JoystickButtonState, ref controller.JoystickClicked);
        }

        public void SimulateButtonDown(ref HVRButtonState buttonState, ref float buttonValue)
        {
            buttonState.JustActivated = true;
            buttonValue = 1f;
            buttonState.Active = true;
        }
        public void SimulateButtonDown(ref HVRButtonState buttonState, ref bool buttonValue)
        {
            buttonState.JustActivated = true;
            buttonValue = true;
            buttonState.Active = true;
        }
        public void SimulateButtonUp(ref HVRButtonState buttonState, ref float buttonValue)
        {
            buttonState.JustActivated = false;
            buttonValue = 0f;
            buttonState.Active = false;
        }
        public void SimulateButtonUp(ref HVRButtonState buttonState, ref bool buttonValue)
        {
            buttonState.JustActivated = false;
            buttonValue = false;
            buttonState.Active = false;
        }
        private void MoveHands()
        {
            Vector3 direction = new Vector3(MouseDelta.x, MouseDelta.y, MouseDeltaScroll.y);

            if (UsingLeftHand)
                _controllerTargetLeft.position += _handMovementSpeed * Time.deltaTime * (_camera.transform.rotation * direction);

            if (UsingRightHand)
                _controllerTargetRight.position += _handMovementSpeed * Time.deltaTime * (_camera.transform.rotation * direction);
        }

        private void RotateHands()
        {
            Vector3 direction = new Vector3(-MouseDelta.y, MouseDelta.x, MouseDeltaScroll.y);

            if (UsingLeftHand)
                _controllerTargetLeft.Rotate(_handsRotationSpeed * Time.deltaTime * direction);

            if (UsingRightHand)
                _controllerTargetRight.Rotate(_handsRotationSpeed * Time.deltaTime * direction);
        }
        private void SimulateGrab(HVRController controller)
        {
            if (controller.Side == HVRHandSide.Left)
            {
                if (_lockLeftHandGrabbable)
                {
                    _lockLeftHandGrabbable = false;
                    return;
                }
            }
            if (controller.Side == HVRHandSide.Right)
            {
                if (_lockRightHandGrabbable)
                {
                    _lockRightHandGrabbable = false;
                    return;
                }
            }

            SimulateGripButton(controller, true);

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

            float[] curlsLeftHand = HVRController.LeftFingerCurls;

            if (ControllerLeft.XRNode == XRNode.RightHand)
            {
                curlsLeftHand = HVRController.RightFingerCurls;
            }

            for (int i = 0; i < 5; i++)
            {
                curlsLeftHand[i] = ControllerLeft.FingerCurls[i];
            }

            float[] curlsRightHand = HVRController.LeftFingerCurls;

            if (ControllerRight.XRNode == XRNode.RightHand)
            {
                curlsRightHand = HVRController.RightFingerCurls;
            }

            for (int i = 0; i < 5; i++)
            {
                curlsRightHand[i] = ControllerRight.FingerCurls[i];
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
