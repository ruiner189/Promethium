# Promethium
**This Mod does NOT work on the Demo. It is for the Early Access version of the Game**

The goal of the mod is to offer balance changes, new mechanics, and QoL features. It is currently in development, so please report any bugs found!

## Localization
Localization is being done at https://docs.google.com/spreadsheets/d/1r7o-GVIn6ljL4DvOIRY4eTTo1OhWjd-P2HyBS9UB0OI/edit#gid=0
If you want to help translate, feel free to!

## Custom Start Deck
If you are using [Custom Start Deck](https://peglin.thunderstore.io/package/bo0tzz/CustomStartDeck/), you can add Promethium's relics by using the names found [here](https://github.com/ruiner189/Promethium/blob/main/Patches/Relics/CustomRelics/CustomRelicEffect.cs)!

## Installation
### Thunderstore (ModManager)
1. Download [Thunderstore](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager)
2. Choose Peglin for the Game Selection
3. Create or select a profile
4. Search "Promethium" in the search bar
5. Click "Download with dependencies"
6. Click "Start Modded" to begin playing!

### Github (Manual)
1. Download [Promethium.dll](https://github.com/ruiner189/Promethium/releases)
2. Download [BepInEx_x64_5.x.x.x.zip](https://github.com/BepInEx/BepInEx/releases)
3. Go to your install directory for Peglin.
    * For Steam go to your Steam Library and right click Peglin > Manage > Browse Local Files
4. Unzip the BepInEx folder into your peglin directory
5. Launch Peglin. BepInEx will then create a plugins folder. Close Peglin after the game launches.
6. Put Promethium in your plugin folder (...\Peglin\Bepinex\plugins)

## Releases
| Peglin - Version| Mod Version | Link |
|----------|-------------|-----------------|
| v0.7.24 | v1.1.7 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.1.7/Promethium.dll) |
| v0.7.21 - v0.7.23 | v1.1.4 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.1.4/Promethium.dll) |
| v0.7.0 - v0.7.20 | v1.1.2 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.1.2/Promethium.dll) |
| v0.7.0 - v0.7.20 | v1.0.10 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.0.10/Promethium.dll) |

## Changelog
<details>
  <summary> v1.1.7 </summary>
     <ul> <li>Added compatibility checks for Endless Peglin and Custom Start Deck. They should now both work with Promethium without strange behavior</li> </ul>
     <ul> <li>Added support to Custom Start Deck so that Promethium's relics work</li> </ul>
     <ul> <li>Pruning relics now gives you a choice, and is now the default on the config</li> </ul>
     <ul> <li>Fixed a bug where Oreb was not being properly loaded in from a save file</li> </ul>
     <ul> <li>Fixed localization being loaded twice</li> </ul>
     <ul> <li>Fixed max health taken from Infernal Ingot being restored when starting another curse run while keeping Infernal Ingot</li> </ul>
     <ul> <li>Fixed a vanilla bug where loading a save file with more than 100 hp would reset it back to 100</li> </ul>
</details>
<details>
  <summary> v1.1.6 </summary>
     <ul> <li>Fixed Localization typo</li> </ul>
</details>
<details>
  <summary> v1.1.5 </summary>
     <ul> <li>Experimental changes on Matroyshka Shell and Sealed Conviction. You can disable these changes in the config</li> </ul>
     <ul> <li>Fixed pachinko relic minigame causing the game from being able to continue if there are no relics left in the relic pool</li> </ul>
     <ul> <li>Fixed curse 5 not increasing the amount of elites correctly</li> </ul>
     <ul> <li>Fixed curse modifiers not being calculated properly if other modifiers bring it less than 0</li> </ul>
     <ul> <li>Localization Update</li> </ul>
</details>
<details>
  <summary> v1.1.4 </summary>
    <ul> <li>Added new relic: Plasma ball</li> </ul>
    <ul> <li>Added hold effects for martrtorbshka and lightningorb</li> </ul>
    <ul> <li>Changed speed up formula</li> </ul>
    <ul> <li>Fixed orbelisk vanilla behavior being restored if the modifications are removed</li> </ul>
    <ul> <li>Localization Update</li> </ul>
</details>


## Quality of Life Changes
* Game speeds up after 10 seconds of sending your orb out. It currently maxes out at 3x the gamespeed.
* You can now retarget after sending your orb out!
* You can turn off enemy turn on reload in the configs! Default is vanilla behavior.
* Confusion is removed while navigating

## New Mechanics
### Armor
Armor acts as a second resource for health. It is used up first, and has unique ways of replenishing it. The amount of armor you currently have is indicated by a status effect.
### Curse
![Curse1](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_One.png)![Curse2](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Two.png)![Curse3](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Three.png)![Curse4](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Four.png)![Curse5](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Five.png)

Are you ready to test your pachinko skills? If you make it to the final boss, continuing the game will start a Cursed run. There are a total of 5 tiers. Can you make it to the end?
Each curse relic will make the enemies stronger in different ways. It'll also give the player a small buff so they might survive...

#### Starting a Curse Run
At the end of your current run (if you defeat the last boss), choose "Restart" to begin or increase your curse level. Each run will give you three choices of curse relics. The negative effect is the same, while you get to choose the benefit. 

Curse runs too easy? Check the configs to make them harder. You can make it so you lose some relics, reset orbs back to four, or even change the health scaling! 

## New Relics
Excluding the curse relics, Promethium currently several new relics:
### Holster
* Removes Discarding and allows you to hold orbs instead.
* Some orbs have special mechanics while held, and many more are to come!
### Plasma Ball
* Zaps up to 3 additional pegs every 5 pegs hit
### Wumbo Belt
* Doubles the size of your orb.
* Has a special interaction with Mini Belt if you have both relics
### Mini Belt
* Halves the size of your orb.
* Has a special interaction with Wumbo Belt if you have both relics
### Kill Button
* Allows you to end your shot early
* Can only be used once per reload

## Relic Changes
### Gardener's Glove
![Garderner's Glove](https://raw.githubusercontent.com/ruiner189/Promethium/main/Docs/Images/GardenerGlove.png)

----------
### Weighted Chip
* Multipliers now cycles through 7 different layouts. Some layouts are rarer to get then others
   * 0.5x, 0.5x, 1x, 1x, 2x
   * 0.25x, 0.25x, 0.5x, 1.5x, 3x
   * 0.25x, 0.5x, 1x, 2x, 2x
   * 1x, 1x, 1x, 1x, 1x
   * 1.25x, 1.25x, 1x, 0.75x, 0.75x
   * 10x, 0x, 0x, 0x, 10x
   * 0x, 0x, 100x, 0x, 0x

## Orb Changes
### Oreb
Oreb has been redone to have a higher impact in the game. Not only is it back, but you can now upgrade it!

Level 1
* Attack: 1 | Crit: 2
* Has a weird bounce
* Every 3 hits a fragment gets sent off
* Fragments last 2 hits before disappearing

Level 2
* Attack: 2 | Crit: 2
* Has a weird bounce
* Every 3 hits a fragment gets sent off
* Fragments can split one time
* After the last split, the next 2 hits will cause it to disappear

Level 3
* Attack: 2 | Crit: 3
* Has a weird bounce
* Every 2 hits a fragment gets sent off
* Fragments can split twice
* After the last split, the next 2 hits will cause it to disappear

### Stone
![Stone](https://raw.githubusercontent.com/ruiner189/Promethium/main/Docs/Images/Stone3.png)

#### Normal Gameplay
Level 1
* Does nothing

Level 2
* Increases Maximum Armor by 3
* Replenishes Armor by 2 every reload

Level 3
* Increases Maximum Armor by 6
* Replenishes Armor by 4 every reload
#### Cruciball Lvl 3
Level 2
* Increases Maximum Armor by 2
* Replenishes Armor by 1 every reload

Level 3
* Increases Maximum Armor by 4
* Replenishes Armor by 2 every reload
----------
### Bouldorb
All Levels
* Restores 10 Armor if discarded
----------
### Orbelisk
![Orbelisk](https://raw.githubusercontent.com/ruiner189/Promethium/main/Docs/Images/Orbelisk3.png)

Level 1
* Attack: 1 | Crit: 3
* Multiplies damage based on current armor (0.08 * currentArmor)
* On discard transfers multiplier to next orb. Removes all armor and damages player on the amount of armor

Level 2
* Attack: 2 | Crit: 5
* Multiplies damage based on current armor (0.10 * currentArmor)
* On discard transfers multiplier to next orb. Removes all armor and damages player on the amount of armor

Level 3
* Attack: 3 | Crit: 7
* Multiplies damage based on current armor (0.12 * currentArmor)
* On discard transfers multiplier to next orb. Removes all armor and damages player on the amount of armor
