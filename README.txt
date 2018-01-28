----------------------------------
Metadata
----------------------------------

Author    : Genetic Madness Team
Objective : This readme serves the purpose of explaining how to use sources from our GGJ 2018 game Freaktory

----------------------------------
Game description
----------------------------------
The goal of the game is to create via genetic transmision of genes the moster that best first the REQUEST TEXT

For more details on the game mechanics visit: https://globalgamejam.org/2018/games/freaktory


----------------------------------
Usage
----------------------------------
The game uses Unity Scriptable Objects for the game data


Each Gene has a specific scriptable objects that indicates it's values and images

The Color gene is special since it also has ColorRules that can be defined

Those can be found under Resources/Genes

------------------
Level Setup
------------------

The main script of the game is Genialogy.cs. 
There is a Genialogy Game GameObject in each level that has the level setup

GenesPerCharacter        - deprecated stuff to ignore
Generations              - number of generations until the result, the generations need to be added and placed manually then this number changed to reflect that
Dominant Genes Available - # of dominant genes to use in the level
Target Gene Values       - the value of each gene the player needs to have in the last descendant to complete the level
                         - an ignore flag on each gene type means that any value is accepted
AlienContainer           - container for all aliens
OverrideGenePicks        - by default ALL 5 genes are used HAT, HEAD, BODY, LEGS, COLOR (in this order)
                         - use this to override the default gene order (you can have less, but NO DUPLICATES ALLOWED)
The rest are links for instantiation

The AlienBoard is where all your aliens need to be placed. THIS SHOULD BE DONE MANUALLY

The first generation of aliens needs to have their initial values SETUP. Also the aliens need to be in ORDER from left to right for corrent generation fusion

------------------
Alien Setup
------------------
An alien contains a set of GeneValues with a numeric value for each Gene. The value = index of the values data in the scriptable objects
For instance Value 0 for the Body Scriptable Object = FatBody image

Color is handled separately but the value reflects the color array. I.E. Value 0 = Red Color

A default value of -1 means nothing should be placed in the slot -> white empty color

ONLY THE FIRST DEPENDANT ALIENS SHOULD HAVE PREDEFINED VALUES; The rest will be generated automatically


------------------
Making a new level
------------------

To make a new level, create or copy a template level and call it Level #levelNumber

Place all aliens manually and do the setup explained previously

IMPORTANT: The is a SettingsScriptableObject under Resources. That specifies the Max Level 12. 
If you add a new level, make sure to extend the max level. Once the max level is reached, it will be replayed upon completion

Add it to the build settings and make sure that #levelNumber in the level name matches it's Unity BUILD INDEX


------------------
Zoom & Scroll
------------------

There is a zoom script (on UI Canvas) and a scroll script (on Background). They have limits to define the scroll and zoom limits of each level
For levels with a lot of generations, zooming and scrolling is ESSENTIAL. THERE IS ONLY SO MUCH SPACE ON THE SCREEN!!!!!

------------------
Misc
------------------

There rest are a bunch of standard scripts for music, level navigation and whatnot. 
If you are curious you can check them out

If you want to add new rules then the Genialogy script is where it's at!
