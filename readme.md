# HVRSimulator

> [!IMPORTANT]
> *Requires Unity 2019 or newer*

HVRSimulator provides a set of components to control the Hurricane/Hexabody VR Rig using your keyboard and mouse. This is ideal for testing your vr interactions without the inconvenience of wearing your VR headset or for testing multiplayer games, especially if you only own one headset.

## How does it work?

HVRSimulator main functionality is divided in two set of components:

1. Body Simulation

   Body simulation is managed by two classes:

   - `HVRBodySimulator`: Controls the player character's movement and rotation using keyboard and mouse inputs. It allows for walking with WASD keys and looking around by holding the right mouse button to rotate the camera and player rig.
   - `HVRHexabodySimulator`: Extends `HVRBodySimulator` for the Hexabody VR system, adapting the movement and rotation controls to work with Hexabody-specific components.
  
2. Hand Simulation

   Hand simulation is managed by the `HVRHandsSimulator` class:

   - Hand Movement: Simulates the movement and rotation of both left and right VR hand controllers using mouse inputs.
   - Controller Inputs: Injects simulated button press data into the Hurricane VR framework to mimic grip, trigger, and other button actions.

## Key Features

- **Plug & Play:** See **#How to Use**.
- **Compatibility:** Works with both the old and new Input Systems.
- **Configurable Keys:** Customize the controls to suit your preferences.
- **Modular Architecture:** Dependency-free design allows you to use only the components you need.
- **Headset Compatibility:** Works with the headset connected (in Editor/PC).
- **On-Screen Controls Guide:** Optional guide for quick and easy iteration.

## How to Install

1. If you have a previous version installed, **delete its folder from your project** to avoid conflicts.

2. You can get the latest `HVRSimulator package` in two ways:

   - Visit the [Releases](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases) page, download the latest HVRSimulator package, and import it into your project.

   - **Using Package Manager**.
     - Open Package Manager.
     - Select "Add package from git URL.".
     - Enter `https://github.com/AdrielCodeops/HurricaneVR-Extensions.git`.

> [!IMPORTANT]
> If you are using Hexabody, remember to import the HexabodyExtension package included within the HVRSimulator package.

## How to Use

**For Hurricane:**

1. Attach the **`HVRBodySimulator`** and **`HVRHandsSimulator`** components to any game object.
2. Drag and drop your Hurricane Rig game object into the rig parameter of both components.

**For Hexabody:**

1. Attach the **`HVRHexabodySimulator`** and **`HVRHandsSimulator`** components to any game object.
2. Drag and drop your Hexabody Rig game object into the rig parameter of both components.

> [!NOTE]
> You can use a single component to simulate just one part of the rig.
> For example, using only **`HVRHandsSimulator`** will allow you to control your hands, but you will not be able to move the player controller until **`HVRBodySimulator`** or **`HVRHexabodySimulator`** is in use.

**Both (Optional):**

1. Attach the **`HVRSimulatorControlsGUI`** to the same game object where you have attached **`HVRHandsSimulator`** and **`HVRBodySimulator`**/**`HVRHexabodySimulator`**.
2. This will display a guide to the controls on the screen when you run the game.

## Default Controls

> [!NOTE]
> The controls listed below are the defaults for a fresh installation.
> The `HVRSimulatorControlsGUI` component provides an on-screen guide that displays the controls in real-time, including any changes you make to the default settings.

### Body

- **Move:** WASD
- **Camera:** Hold Right Mouse Button
- **Jump [Hexabody Only]:** Space
- **Crouch [Hexabody Only]:** Z/X

### Left Hand

- **Move/Use:** Q
- **Move Forward/Backward:** Q + Scroll Wheel
- **Rotate:** Q + Middle Mouse Button
- **Grip:** Q + G
- **Trigger:** Q + Left Click
- **Primary Button:** Q + 1
- **Secondary Button:** Q + 2
- **Joystick Button:** Q + 3

### Right Hand

- **Move/Use:** E
- **Move Forward/Backward:** E + Scroll Wheel
- **Rotate:** E + Middle Mouse Button
- **Grip:** E + G
- **Trigger:** E + Left Click
- **Primary Button:** E + 1
- **Secondary Button:** E + 2
- **Joystick Button:** E + 3

## Changelog

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_2.0.0) [2.0.0]

```md
# Added
- Added Assembly Definition to Hurricane's code.
- Added package.json so you can install this package from: Package Manager -> Add package from git URL.

# Changed
- Now hexabody integration comes as a separate unity package inside the HVRSimulator package.

# Fixed
- Fixed grabbables sometimes getting stuck in your hand.

# How to update (!)
This version contains breaking changes. Please, follow the instructions in #How To Install.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_1.5.3) [1.5.3]

```md
# Added
- Support for slowing down hand movement/rotation by pressing shift to `HVRHandsSimulator` (configurable).

# Changed
- Made button simulation more robust.
- General code cleanup.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_1.5.2) [1.5.2]

```md
# Changed
- Increased default body rotation speed.

# Fixed
- Body rotation stuttering.
- Camera vertical movement now updates in `LateUpdate` to avoid stuttering.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_1.5.1) [1.5.1]

```md
# Fixed
- UI not working for the right hand.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_1.5) [1.5.0]

```md
# Added
- Support for interacting with UI.

# Changed
- Rewrote the button simulation, now correctly running across the entire Hurricane Framework.
- Cleaned up the implementation of finger curling to improve performance.

# Fixed
* Buttons (A,B, Joystick press) not working correctly.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_1.4.1) [1.4.1]

```md
# Fixed
- Compiler errors in Unity 2019 by removing IDE0090 syntax sugar.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator_1.4) [1.4.0]

```md
# Fixed
- Compiler errors when using old input system.
```

### [HVRSimulator](https://github.com/AdrielCodeops/HurricaneVR-Extensions/releases/tag/Simulator) [1.3.0]

```md
# Changed
- Namespace changed to `HurricaneVRExtensions.Simulator`.
- General code cleanup.

# Fixed
- `AutoResolveDependencies` not finding the hexabody dependencies.

# How to update from 1.2.0
Package root folder has been changed to `HurricaneVRExtensions/Simulator` to better match Hurricane's folder naming. Ensure to remove the old simulator folder at `HurricaneExtensions/Simulator`.
```

### HVRSimulator [1.2.0]

```md
# Added
- New setup procedure: 
- Use `HVRBodySimulator` for Hurricane.
- Use `HVRHexabodySimulator` for Hexabody. 

# Changed
- Separated `HVRBodySimulator` into two components: `HVRBodySimulator` (for Hurricane) and `HVRHexabodySimulator` (for Hexabody).

# Fixed 
- `TrackedPoserDriver` dependency error when simulator components were not attached to the Rig gameobject. 

# How to update from <1.2.0
If you use Hexabody and want to update: Swap `HVRBodySimulator` for `HVRHexabodySimulator`.
```

### HVRSimulator [1.1.0]

```md
# Added 
- Hand forward/backward and rotation controls to `HVRSimulatorControlsGUI`. 

# Fixed
- Hand's finger curls now update when simulating input, even if no object is grabbed by the hand. 
- Typo in `HVRSimulatorControlsGUI
```

### HVRSimulator [1.0.0]

```md
# Added 
- Initial release.
```
