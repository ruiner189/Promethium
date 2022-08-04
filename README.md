# Promethium
**This Mod does NOT work on the Demo. It is for the Early Access version of the Game**

The goal of the mod is to offer balance changes, new mechanics, and QoL features. It is currently in development, so please report any bugs found!

## Localization
Localization is being done at https://docs.google.com/spreadsheets/d/1r7o-GVIn6ljL4DvOIRY4eTTo1OhWjd-P2HyBS9UB0OI/edit#gid=0
If you want to help translate, feel free to! Want to become an editor on the sheet? Please send a pull request on github.

## Dependencies
For this mod to work, you must also have ProLib installed. Download links: [[Github](https://github.com/ruiner189/ProLib/releases)] [[Thunderstore](https://peglin.thunderstore.io/package/Promethium/ProLib/)]

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
| v0.7.45 | v1.3.0 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.3.0/Promethium.dll) |
| v0.7.34 | v1.2.4 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.2.4/Promethium.dll) |
| v0.7.26 | v1.1.8 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.1.8/Promethium.dll) |
| v0.7.21 - v0.7.23 | v1.1.4 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.1.4/Promethium.dll) |
| v0.7.0 - v0.7.20 | v1.1.2 | [Download](https://github.com/ruiner189/Promethium/releases/download/1.1.2/Promethium.dll) |

## Changelog
<details>
  <summary> v1.3.0 </summary>

* Split Promethium into two mods - one with content and the other as a library
* Updated to v0.7.45
* Added support to Custom Challenges
* Bug fixes

</details>
<details>
  <summary> v1.2.4 </summary>

* Attempt two at fixing bombs showing a bounce prediction when popped
</details>

<details>
  <summary> v1.2.3 </summary>

* Fixed slimy pegs showing a bounce prediction when popped
* Fixed bombs showing a bounce prediction when popped
* Lowered Orb of Greed self-damage 
  * **Lvl 2**: 3 -> 1
  * **Lvl 3**: 5 -> 3
* Added a config option to use the default prediction system instead of Promethium's
* Moved custom orbs to their own section in the config
</details>

<details>
  <summary> v1.2.2 </summary>

* Fixed Localization not loading properly
</details>

<details>
  <summary> v1.2.0 </summary>

* Updated to v0.7.34
* New QoL Feature: Dynamic Relic Icons
* New Relics: Chaos, Order, Pocket Moon, Reality Marble
* Prediction System overhauled
* Now compatible with other mods that add orbs
* Fixed integer overflow
* Localization now auto-updates!
* Nerfed Orb of Greed (Now damages player on discard)
</details>

<details>
  <summary> v1.1.8 </summary>

* Speed-up only activates when you are not using the in-game speed-up
* Speed-up config is now defaulted to off
* Added new orb: Orb of Greed
* Added custom orb support to Custom Start Deck
</details>

<details>
  <summary> v1.1.7 </summary>

* German localization by Denny
* Added compatibility checks for Endless Peglin and Custom Start Deck. They should now both work with Promethium without strange behavior
* Added support to Custom Start Deck so that Promethium's relics work
* Pruning relics now gives you a choice, and is now the default on the config
* Fixed a bug where Oreb was not being properly loaded in from a save file
* Fixed localization being loaded twice
* Fixed max health taken from Infernal Ingot being restored when starting another curse run while keeping Infernal Ingot
* Fixed a vanilla bug where loading a save file with more than 100 hp would reset it back to 100
</details>

<details>
  <summary> v1.1.6 </summary>

* Fixed Localization typo
</details>
<details>
  <summary> v1.1.5 </summary>

* Experimental changes on Matroyshka Shell and Sealed Conviction. You can disable these changes in the config
* Fixed pachinko relic minigame causing the game from being able to continue if there are no relics left in the relic pool
* Fixed curse 5 not increasing the amount of elites correctly
* Fixed curse modifiers not being calculated properly if other modifiers bring it less than 0
* Localization Update
</details>

<details>
  <summary> v1.1.4 </summary>

* Added new relic: Plasma ball
* Added hold effects for martrtorbshka and lightningorb
* Changed speed up formula
* Fixed orbelisk vanilla behavior being restored if the modifications are removed
* Localization Update
</details>


## Quality of Life Changes
* Game speeds up after 10 seconds of sending your orb out. It currently maxes out at 3x the gamespeed.
* You can now retarget after sending your orb out!
* You can turn off enemy turn on reload in the configs! Default is vanilla behavior.
* Confusion is removed while navigating
* You can now set relic icons to disappear if they are not relevant to what you are currently doing (I.e. relics that don't affect navigation won't appear while navigating). This is mainly aimed for cursed/endless runs, as your screen just becomes all relics. This feature requires you to have a minimum number of relics to activate, and can be turned off.
* Prediction system is now much more accurate with pegnet and unicorn horn. There are still some edge cases, but generally should be accurate.

## New Mechanics
### Armor
Armor acts as a second resource for health. It is used up first, and has unique ways of replenishing it. The amount of armor you currently have is indicated by a status effect.
### Curse
![Curse1](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_One.png)![Curse2](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Two.png)![Curse3](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Three.png)![Curse4](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Four.png)![Curse5](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Five.png)

Are you ready to test your pachinko skills? If you make it to the final boss, continuing the game will start a Cursed run. There are a total of 5 tiers. Can you make it to the end?
Each curse relic will make the enemies stronger in different ways. It'll also give the player a small buff so they might survive...

#### Starting a Curse Run
At the end of your current run (if you defeat the last boss), choose "Restart" to begin or increase your curse level. Each run will give you three choices of curse relics. The negative effect is the same, while you get to choose the benefit. 

Curse runs too easy? Check the configs to make them harder.

## New Relics
Excluding the curse relics, Promethium currently several new relics:
<details>
  <summary> Holster </summary>

  * Removes Discarding and allows you to hold orbs instead.
  * Some orbs have special mechanics while held, and many more are to come!
</details>

<details>
  <summary> Plasma Ball </summary>

  * Zaps up to 3 additional pegs every 5 pegs hit
</details>

<details>
  <summary> Wumbo Belt </summary>

  * Doubles the size of your orb.
  * Has a special interaction with Mini Belt if you have both relics
</details>

<details>
  <summary> Mini Belt </summary>

  * Halves the size of your orb.
  * Has a special interaction with Wumbo Belt if you have both relics
</details>

<details>
  <summary> Kill Button </summary>

  * Allows you to end your shot early
  * Can only be used once per reload
</details>

<details>
  <summary> Chaos </summary>

  * Removes rarity from all relics. That means you can get a boss relic from a chest and a common relic from a boss!
</details>

<details>
  <summary> Order </summary>

  * You are more likely to find similar Relics.

</details>

<details>
  <summary> Pocket Moon </summary>

  * Reduces gravity
</details>

<details>
  <summary> Reality Marble </summary>

  * Changes the direction of gravity every x seconds
</details>

## Relic Changes
<details>
  <summary> Gardener's Glove </summary>

  * Gardener's Glove now increases your maximum armor by 5, and regenerates 1 armor per turn.
</details>

<details>
  <summary> Sealed Conviction </summary>

  * Removes all discards. Damage is increased based on number of discards removed.
</details>

<details>
  <summary> Matryoshka Shell </summary>

  * Damage reduction is now percentage based instead of the flat decrease.
</details>

<details>
  <summary> Weighted Chip </summary>

  * Multipliers now cycles through 7 different layouts. Some layouts are rarer to get than others
    * 0.5x, 0.5x, 1x, 1x, 2x
    * 0.25x, 0.25x, 0.5x, 1.5x, 3x
    * 0.25x, 0.5x, 1x, 2x, 2x
    * 1x, 1x, 1x, 1x, 1x
    * 1.25x, 1.25x, 1x, 0.75x, 0.75x
    * 10x, 0x, 0x, 0x, 10x
    * 0x, 0x, 100x, 0x, 0x
</details>

## New Orbs
<details>
<summary> Orb of Greed </summary>

A new orb that adds more deck management to your loadout.

Level 1
* Attack: 1 | Crit: 1
* Shuffles Deck on discard
* Can only be discarded once per battle

Level 2
* Attack: 1 | Crit: 1
* Shuffles Deck on discard
* Prevents enemy turn on discard
* Can only be discarded once per battle

Level 3
* Attack: 1 | Crit: 1
* Adds a crit peg and refresh peg to the board
* Shuffles deck on discard
* Prevents enemy turn on discard
* Can only be discarded once per battle
</details>

## Orb Changes
<details>
<summary> Oreb </summary>

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
</details>

<details>
<summary> Stone </summary>

### Normal Gameplay
Level 1
* Does nothing

Level 2
* Increases Maximum Armor by 3
* Replenishes Armor by 2 every reload

Level 3
* Increases Maximum Armor by 6
* Replenishes Armor by 4 every reload
### Cruciball Lvl 3
Level 2
* Increases Maximum Armor by 2
* Replenishes Armor by 1 every reload

Level 3
* Increases Maximum Armor by 4
* Replenishes Armor by 2 every reload
</details>

<details>
<summary> Bouldorb </summary>

All Levels
* Restores 10 Armor if discarded
</details>

<details>
<summary> Orbelisk </summary>

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
</details>