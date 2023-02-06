# Icons Creator

## About :information_source:

This tool can create icons of any 3D objects imported in Unity. This includes prefabs, models, objects from loaded scenes, also you can make icons out of the entire folder contatining 3D objects.

The tool was initially created for my game, but it was way too basic and required a lot of manual work to do to create icons. Also it used Odin for custom inspector and I wanted to learn more about editor coding, so I took the initial version of the tool, improved it, removed any external dependencies, and decided to make it public.

## Getting started :rocket:

### Compatibility

| Render Pipeline &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;	| Compatible 					|
| :----------- 			| :-----------: 			|
| Built-In    			| :white_check_mark: 	|
| Universal   			| :white_check_mark: 	|
| High Defenition   | :x: 								|

If you are using URP, please make sure the depth texture is enabled.

### Important notes

- The tool uses a special scene named Icons_Creation, it comes with the package. It is used to set the objects and render them to the icons. Please don't do anything with this scene, just ignore it. If you accidentally modified it - delete it, and the tool will regenerate it.
- Please notice that the tool does load, set active, and unload a special scene every time it creates an icon. So, if you are using EditorSceneManager events, keep it in mind.

### Install

1. Download the latest release
2. Import it in the Unity

## Future :crystal_ball:

I'm not sure if I will be working on this tool anymore, but if I will, I am probably going to add some of the following features:
- Frame
- Alpha mask
- Perspective camera projection
- Ambient lighting
- Object outline
