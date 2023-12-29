# PlayerAvatarLoader v1.0.0
### Uses ModelReplacementAPI to load a number of unique models for each player

## Features
- Allows each player to have up to 4 unique models (1 for each vanilla suit).
- 4 default avatars are for anyone who does not have a unique asset bundle or for any suits someone's bundle does not have.
  - Orange suit: Goku (Xenoverse 2)
  - Green suit: WarGreymon (Digimon Story: Cyber Sleuth)
  - Hazard suit: Burger King (Sneak King)
  - Pajama suit: Freddy (Five Nights at Freddy's: Help Wanted)
- Works with More_Suits to remove the limit of 4 models. To use, edit this mod's config file to include ALL of the additional suit names you have loaded.

## Base Instructions
- Create your asset bundle by following the ModelReplacementAPI Unity workflow HERE: https://github.com/BunyaPineTree/LethalCompany_ModelReplacementAPI/wiki/Using-the-Unity-Workflow
- Add every model you wish to have into one scene.
- Set every model's "Asset Bundle Name" to YOUR Steam64 ID.
- Set each model's "Asset Name" to the suit name you want to replace. (Ex: To replace the default orange suit you should name it "Orange suit", but to replace the hazard suit you should name it "Hazard suit".)
- Build the asset bundle and check the bundle's .Manifest to be sure all of your assets are present.
- Share your asset bundle with all players using the mod.
- Copy everyone's bundle into the "Avatars" folder inside the mod installation directory.

## Using Blend Shapes
- Initial setup:
  - Create an XML file that's name is your Steam64 ID inside of this mod's "Avatars" folder.
  - Structure the XML nodes as follows: root > Suitname > BlendShapes > Default. (Suitname is the name of your suit without any spaces)
  - Inside of Default, create as many nodes as you have blendshapes that you wish to use. Name each node Blend1, Blend2, Blend3 etc...
  - Inside of each Blend(X) node set the inner text to "Index%/Value". Index should be the index of the blendshape in unity, Value should be the default value you want the blendshape to be (Ex: 0%/50, would use index 0 at value 50).

- Set Emote Values
  - Inside of the BlendShapes node, create an Emote(X) node for each emote you want to add blendshapes to. (X) should be replaced with "1" to be used for the dance emote, or "2" for the point emote.
  - Inside of Emote(X), create as many nodes as you have blendshapes that you wish to alter. Name each node Blend1, Blend2, Blend3 etc...
  - Inside of each Blend(X) node set the inner text to "Index%/Value". Index should be the index of the blendshape in unity, Value should be the new value you want the blendshape to be when performing the emote (Ex: 0%/50, would use index 0 at value 50).
  - NOTE: to prevent errors, all blendshapes used by emotes should ALSO be in the Default node.
 
## Using Dynamic Bones
### DISCLAIMER:
Dynamic Bones cannot be added to the mod's release as it is a paid unity asset. However, for those who own Dynamic Bones and wish to make use of it, the mod does support using it with some added work.

## Screenshots
![Goku](https://github.com/NotAustinVT/LethalCompany_PlayerAvatarLoader/blob/main/Screenshots/Goku.png?raw=true)

![WarGreymon](https://github.com/NotAustinVT/LethalCompany_PlayerAvatarLoader/blob/main/Screenshots/WarGreymon.png?raw=true)

![Burger King](https://github.com/NotAustinVT/LethalCompany_PlayerAvatarLoader/blob/main/Screenshots/Burger%20King.png?raw=true)

![Freddy](https://github.com/NotAustinVT/LethalCompany_PlayerAvatarLoader/blob/main/Screenshots/Freddy.png?raw=true)

## CHANGELOG
- v1.0.0
  - Inital Release

- v1.0.1
  - Fixed Folder Structure
 
- v1.1.0
  - Added ability to read XML files
  - Added support for custom blendshapes when using vanilla emotes (see Using Custom BlendShapes in the mod's wiki for a tutorial
  - *Added support for Dynamic Bones (see "Using Dynamic Bones" in the mod's wiki for more info as well as a tutorial)
