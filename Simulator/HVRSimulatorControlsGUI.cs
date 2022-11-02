using UnityEngine;

namespace HurricaneVRExtensions.Simulator
{
    public class HVRSimulatorControlsGUI : MonoBehaviour
    {
        private HVRBodySimulator _bodySimulator;
        private HVRHandsSimulator _handsSimulator;

        private void Awake()
        {
            if (GetComponent<HVRBodySimulator>() != null)
                _bodySimulator = GetComponent<HVRBodySimulator>();

            if (GetComponent<HVRHandsSimulator>() != null)
                _handsSimulator = GetComponent<HVRHandsSimulator>();
        }
        public void OnGUI()
        {
            if (_bodySimulator && _bodySimulator.enabled)
                RenderBodySimulatorTutorial();

            if (_handsSimulator && _handsSimulator.enabled)
                RenderHandsSimulatorTutorial();
        }

        private void RenderBodySimulatorTutorial()
        {
            float y = 275;
            if (!_handsSimulator || !_handsSimulator.enabled)
                y = 100;

            string tutorialText =
@"Move -> WASD
Rotate Camera -> Hold Mouse right click
Jump (Hexabody) -> Space
Crouch (Hexabody) -> Z/X";

            GUI.BeginGroup(new Rect(1, Screen.height - y, 300, 100));

            GUI.Box(new Rect(0, 0, 100, 25), "<b>Body controls</b>");
            GUI.TextArea(new Rect(0, 25, 300, 100), string.Format(tutorialText));

            GUI.EndGroup();
        }

        private void RenderHandsSimulatorTutorial()
        {
            string title = "<b>Hands controls</b>";
            string tutorialText =
@"Move Left hand -> {0}
Move Right hand -> {1}";
            string grabOrRelease = "Grab";
            bool leftHandTutorial = false;
            bool rightHandTutorial = false;

            if (_handsSimulator.UsingLeftHand)
            {
                leftHandTutorial = true;
                grabOrRelease = (_handsSimulator.HandGrabberLeft.IsGrabbing) ? "Release" : "Grab";
                title = "<b>Left hand controls</b>";
                tutorialText =
    @"While holding -> {0}
Move forward/backward -> Scroll Wheel
Rotate -> Middle mouse button
{1} -> {2}
Trigger -> Mouse left click
Primary button -> {3}
Secondary button -> {4}
Joystick button -> {5}";
            }

            if (_handsSimulator.UsingRightHand)
            {
                rightHandTutorial = true;
                grabOrRelease = (_handsSimulator.HandGrabberRight.IsGrabbing) ? "Release" : "Grab";
                title = "<b>Right hand controls</b>";
                tutorialText =
    @"While holding -> {0}
Move forward/backward -> Scroll Wheel
Rotate -> Middle mouse button
{1} -> {2}
Trigger -> Mouse left click
Primary button -> {3}
Secondary button -> {4}
Joystick button -> {5}";
            }

            GUI.BeginGroup(new Rect(1, Screen.height - 175, 300, 250));
            GUI.Box(new Rect(0, 0, 150, 25), title);

            if (leftHandTutorial)
            {
                GUI.TextArea(new Rect(0, 25, 300, 150), string.Format(tutorialText,
                                                      _handsSimulator.LeftHandKey.ToString(),
                                                      grabOrRelease,
                                                      _handsSimulator.GripKey.ToString(),
                                                      _handsSimulator.PrimaryButtonKey.ToString(),
                                                      _handsSimulator.SecondaryButtonKey.ToString(),
                                                      _handsSimulator.JoystickButtonKey.ToString()));
            }
            else if (rightHandTutorial)
            {
                GUI.TextArea(new Rect(0, 25, 300, 150), string.Format(tutorialText,
                                                      _handsSimulator.RightHandKey.ToString(),
                                                      grabOrRelease,
                                                      _handsSimulator.GripKey.ToString(),
                                                      _handsSimulator.PrimaryButtonKey.ToString(),
                                                      _handsSimulator.SecondaryButtonKey.ToString(),
                                                      _handsSimulator.JoystickButtonKey.ToString()));
            }
            else
            {
                GUI.TextArea(new Rect(0, 25, 300, 150), string.Format(tutorialText, _handsSimulator.LeftHandKey.ToString(), _handsSimulator.RightHandKey.ToString()));
            }
               

            GUI.EndGroup();
        }
    }
}
