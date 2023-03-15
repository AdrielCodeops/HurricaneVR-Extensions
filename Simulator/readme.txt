
HVRSimulator: This tool provides components to control the hurricane/hexabody rig with the mouse/keyboard.

* SETUP:

Hurricane: Attach HVRBodySimulator & HVRHandsSimulator to any gameobject (You can also add only one component, to simulate only the body or hands.).
then feed Rig parameter with your hurricane rig. Done <3

Hexabody: Attach **HVRHexabodySimulator** & HVRHandsSimulator to any gameobject (You can also add only one component, to simulate only the body or hands.).
then feed Rig parameter with your hexabody rig. Done <3

* Optional: Attach HVRSimulatorControlsGUI to the same gameobject that you have attached HVRHandsSimulator & HVRBodySimulator/HVRHexabodySimulator.
It will display a guide to the controls on the screen when you run the game.

----------------------------------------
Features:
- Plug&Play, no code or setup required: Attach HVRBodySimulator/HVRHexabodySimulator and HVRHandsSimulator to any gameobject, then feed Rig parameter with hexabody/hurricane root object. Done :)
- Compatible with Old & New Input System.
- Configurable Keys.
- Dependency free, no third party assets required. (Aside from Hurricane/Hexabody ofc :P)
- Modular architecture, never inherits or modifies existing hurricane code.
- Works with the headset connected (On Editor/PC).
- Optional component: HVRSimulatorControlsGUI that provides an on-screen controls guide.

Controls: (Note that if you modify the keys of the controls, the controls will change.)
Body:
- Move -> WASD
- Camera -> Hold Right mouse click
- Jump (Hexabody)-> Space
- Crouch (Hexabody)-> Z/X

Left Hand:
- Move/Use -> Q
- Move forward/backward -> Q + Scroll wheel
- Rotate -> Q + Middle mouse button
- Grip -> Q + G
- Trigger -> Q + Left Click
- Primary Button -> Q + 1
- Secondary Button -> Q + 2
- Joystick Button -> Q + 3

Right Hand:
- Move/Use -> E
- Move forward/backward -> E + Scroll wheel
- Rotate -> E + Middle mouse button
- Grip -> E + G
- Trigger -> E + Left Click
- Primary Button -> E + 1
- Secondary Button -> E + 2
- Joystick Button -> E + 3

Changelog ------------------------------
HVRSimulator 1.5.1
- Fixed UI not working for right hand.

HVRSimulator 1.5
- Change: Complete rewrite of the button simulation, now correctly running across the entire Hurricane Framework.
- Change: You can now interact with UI elements with simulated controllers.
- Change: Cleaned up the implementation of finger curling to improve its performance.
- Fix: The simulation of tap buttons (A, B, Joystick...) on the controller now works correctly.

HVRSimulator 1.4.1
- Fix: Removed IDE0090 syntax to avoid compiler errors on Unity 2019

HVRSimulator 1.4
- Fix: Compiler errors when using old input system

HVRSimulator 1.3
- Fix: AutoResolveDependencies stability.
- Change: Improved code self-documentation.
- Change: Namespace changed to "HurricaneVRExtensions.Simulator"

Update Considerations: Package root folder changed to "HurricaneVRExtensions/Simulator" to better match hurricane's folder naming. - Keep sure to remove old simulator folder at "HurricaneExtensions/Simulator".

HVRSimulator 1.2
NEW SETUP PROCEDURE: If using Hurricane use HVRBodySimulator, if using Hexabody use HVRHexabodySimulator.

- Fixed TrackedPoserDriver dependency error when HVRSimulator components were not attached to the Rig gameobject.
- HRVBodySimulator has been separated into two components: HVRBodySimulator (For Hurricane) & HVRHexabodySimulator (For Hexabody). So now you just have to remove the HVRHexabodySimulator script in case you do not have imported or use hexabody.
If you use hexabody and want to update: Swap HVRBodySimulator for HVRHexabodySimulator.

HVRSimulator 1.1
- Hand's finger curls now update when simulating input. (Finger animations are now executed with the input, even if no object is grabbed by the hand)
- Added hand forward/backward and rotation controls to the guide.
- Fixed a typo in the controls guide.
- Improved readme.

HVRSimulator 1.0
- Release
