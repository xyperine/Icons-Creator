# Icons Creator 0.2.3

## Table of contents
* [About :information_source:](#about-information_source)
* [Getting started :rocket:](#getting-started-rocket)
  * [Compatibility](#compatibility)
    * [Rendering](#rendering)
    * [Editor version](#editor-version)
  * [Important notes](#important-notes)
  * [Install](#install)
* [Examples :eyes:](#examples-eyes)
* [Future :crystal_ball:](#future-crystal_ball)
* [For developers :wrench:](#for-developers-wrench)
  * [Documentation](#documentation)
  * [Testing](#testing)

## About :information_source:

This tool can create icons of any 3D objects imported in Unity: prefabs, models, objects placed on opened scenes. Also you can make icons out of entire folders contatining 3D objects.

This is kind of experimental tool, I did it to learn more about tooling and editor coding. It is based on a tool that I originally created for a game I am working on, but it was quite limited and required a lot of manual work to do to create icons. So I took the initial version of the tool, improved it, removed any external dependencies, and decided to make it public.

## Getting started :rocket:

### Compatibility

#### Editor version

It should be fine if you are using 2021.3+, however, I didn't really test any other versions.

*I will update this section if I will try other editor versions.*

#### Rendering

| Render Pipeline &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;	| Compatible 					|
| :----------- 			| :-----------: 			|
| Built-In    			| :white_check_mark: 	|
| Universal   			| :white_check_mark: 	|
| High Defenition   | :x: 								|

If you are using URP, please make sure the depth texture is enabled.

### Important notes

- The tool can only be used in Edit mode.
- The tool is using a special scene named `Icons_Creation`, it comes with the package. It is used to set the objects and render them as icons. Please don't do anything with this scene, just ignore it. If you accidentally modified it - delete it, and it will be regenerated next time you open the tool window.
- Please note that the tool does load, set active, and unload a special scene every time it creates an icon. So, if you are using `EditorSceneManager` events, keep it in mind.

### Install

1. Download the .unitypackage file from the [latest release](https://github.com/xyperine/Icons-Creator/releases/tag/v0.2.3)
2. Import it to Unity

## Examples :eyes:
Here are some examples with different backgrounds:

Ambulance (none) | Blaster (none)  | Chair (color)    | Hot dog (color)  | House (texture)    | Turret (texture) 
:---------|:--------:|:--------:|:--------:|:--------:|:--------:
![alt text](https://github.com/xyperine/Icons-Creator/blob/main/Assets/Plugins/IconsCreator/Samples/Textures/Icons/Ambulance_Icon.png?raw=true)|![alt text](https://github.com/xyperine/Icons-Creator/blob/main/Assets/Plugins/IconsCreator/Samples/Textures/Icons/Blaster_Icon.png?raw=true)|![alt text](https://github.com/xyperine/Icons-Creator/blob/main/Assets/Plugins/IconsCreator/Samples/Textures/Icons/Chair_Icon.png?raw=true)|![alt text](https://github.com/xyperine/Icons-Creator/blob/main/Assets/Plugins/IconsCreator/Samples/Textures/Icons/Hot_Dog_Icon.png?raw=true)|![alt text](https://github.com/xyperine/Icons-Creator/blob/main/Assets/Plugins/IconsCreator/Samples/Textures/Icons/House_Icon.png?raw=true)|![alt text](https://github.com/xyperine/Icons-Creator/blob/main/Assets/Plugins/IconsCreator/Samples/Textures/Icons/Turret_Icon.png?raw=true)


## Future :crystal_ball:

I'm not sure if I will be working on this tool anymore, but if I will, I am probably going to do some QoL updates, improve code, optimize it, and add some or all of the following features:

- Frame
- Alpha mask
- Perspective camera projection
- Ambient lighting
- Object outline

## For developers :wrench:

If you want to fork and modify the tool, this information may be useful.

### Documentation

There is no actual documentation for the code at the moment, I will update this section if I will make one. So here is a short and simple explanation of how does this tool work:

There are two main classes:

- `IconsCreatorWindow` - handles all GUI drawing and saving properties.
- `IconsCreator` - coordinates low-level components mentioned below to create icons.

Classes that do the actual work:

- `IconsCreatorInternalSceneHandler` - generates an internal scene named `Icons_Creation`, loads it, closes it, and places the objects on the internal scene.
- `IconsCreatorCameraUtility` - adjusts camera position, rotation, orthographic size and provides camera view texture.
- `IconsSaver` - saves camera view as a sprite asset.

Some details:

- Every time the tool window is opened, `IconsCreator` asks `IconsCreatorInternalSceneHandler` to create the internal scene if it is missing.
- To draw a preview `IconsCreator` makes `IconsCreatorInternalSceneHandler` and `IconsCreatorCameraUtility` work together to place the first object from the `Objects` list on the internal scene and retrieve the camera view from that scene.
- When creating icons, the "preview" procedure mentioned above is applied for every object from the `Objects` list. Then `IconsSaver` saves every retrieved camera view texture as a sprite asset with all user specified properties to the `Assets/Textures/Icons` folder created by the tool.

*I am not really satisfied with the code in the project, so I will probably refactor it and update this section.*

### Testing

I made some tests to make sure that the assets the tool is generating are meeting certain requirements. The tests don't cover content validity of the generated icons.

Before you run tests make sure `Icons_Creation` scene is present and if you accidently modified it - delete the scene, then reopen the tool window and the scene will be automatically regenerated.
