# MiniGameParade
An HTC Vive Game that encapsulates several mini-simulations that have been built up as part of the [FusionEd LiveStreams](https://www.youtube.com/c/Fuseman). The goal of this project is to showcase all these mini games in a cohesive project that is fun and inspiring for people to make their own games / content. The project is divided into two components: the Framework and the Mini-Games.

# Update NOTES
1. When updating the SteamVR SDK, makes sure **NOT** to overwrite SteamVR_LoadLevel.cs. That code has been customized to to properly load levels **ASYNC** and customized to use level ids as opposed to a string name.
2. Other frameworks that we use are [QuickSheets](https://github.com/kimsama/Unity-QuickSheet), [Oculus Audio SDK](https://developer3.oculus.com/documentation/audiosdk/latest/), and [VRTK](https://github.com/thestonefox/VRTK). Each of these frameworks / plugins will need to be periodically updated. 

# Framework

The Framework is built from several singleton classes that keep track of the GameState, the Player, small Activites, Transportation, Audio, and the Room. Each of these managers communicates with each other to make sure transitions between games are seamless and not jarring. 

## Game Manager

The Game Manager is designed to keep track of the current MiniGame (denoted with the MiniGameManager) and transition between them. It is the central location for all components to talk with in order get access to the current **state** of the game.

## Player Manager

The Player Manager is designed to keep track of the CameraRig and the HandControllers. It is a central place to get access to the global player. Whenever a mini game is entered, the references update to the CameraRig in the new scene. This is useful to get access to the Global Left and Right Hands of the Player. It also will enable common gesture interactions for menus or swapping hands in the future. It also contains information of the player's play space.

## Audio Manager

The Audio Manager is in charge of playing audio throughout the whole experience. This is ***extremely useful*** in the case where you want to gradually fade in/out sound between scenes. In most cases, you will be able to use the **PlayAudio.cs** script to play an audio clip and it will work very similarly to an Audio Source. 

## Transportation Manager

The Transportation Manager is in charge of the Box and controlling its animation state. It also interacts with the Player Manager to get the location of the player and position the Box accordingly. The reason this is called the Transportation Manager instead of the Box Manager is that this is solely responsbile for the transportation and not activites that can happen inside the box. 

## Room Manager

The Room Manager is in charge of providing the player instructions and activities to do while in the transition loading period between scenes. Currently, we have the Simon Says activity implemented for players to do while loading.

## Mini-Game Manager

Each Mini-Game **MUST** extend from this manager to be supported by the whole flow of the mini-games. By extending this class, most of the hooks are already made with the Game Manager to transition into and out off your mini-game scene. The only thing that is required off a mini-game child class is that it must provide an end state for the game, whether that be time, death, or anything else. 

## Interactable Objects

As part of the framework, we provide two scripts InteractableObject.cs and ControllerCollider.cs. Both these scripts are used to interact with objects and bring them between worlds (currently unimplemented). The goal is to make it easy to have objects move based on hand movements and extremely easy to pick up and release. To use them, a script must extend the InteractObject class and the scene must contain ControllerCollider objects that are associated with a given hand (the HandControllerPrefab is included in the Framework folder). Once that is setup, you can easily pick up objects with the trigger and drop them with the Grip.  

# Mini-Games

Current we have three mini-games: Robin Hood, Asteroid Mining, and Operation. 

Activities that we want to support for the box in the future include: legos / block stacking, scrabble, bop-it, darts, cards, dominos. Further ideas from Please, Don't touch anything / common board games. 

For the future, we could also use these mini-games as a basis for a Mario Party style game.
