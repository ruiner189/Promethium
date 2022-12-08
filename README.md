# Promethium
**This Mod does NOT work on the Demo. It is for the Early Access version of the game**

The goal of the mod is to offer balance changes, new mechanics, and QoL features. It is currently in development, so please report any bugs found!

## Localization
Localization is being done at https://docs.google.com/spreadsheets/d/1r7o-GVIn6ljL4DvOIRY4eTTo1OhWjd-P2HyBS9UB0OI/edit#gid=0
If you want to help translate, feel free to! Want to become an editor on the sheet? Please send a pull request on github.

## Dependencies
For this mod to work, you must also have ProLib installed. Download links: [[Github](https://github.com/ruiner189/ProLib/releases)] [[Thunderstore](https://peglin.thunderstore.io/package/Promethium/ProLib/)]

## Save Warning
This mod uses ProLib which does affect your save file. Please back up your save file before using.

## Installation
### Thunderstore 
1. Download [Thunderstore](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager)
2. Choose Peglin for the Game Selection
3. Create or select a profile
4. Search "Promethium" in the search bar
5. Click "Download with dependencies"
6. Click "Start Modded" to begin playing!

## Changelog
<details>
   <summary> v1.5.1 </summary>
   
* Updated localization file
</details>


<details>
   <summary> v1.5.0 </summary>
   
* Updated to Peglin 0.8.10
* Added a new orb-type: Potions.
* Potions are a "consumable". Unless stated, they do not take up a turn to be used.
* Holster ability to hold orbs has been set to always be available. This is to allow more freedom with your deck, especially with much larger decks due to potions.
* Holster has been removed temporarily. It will be back in the near future, better and stronger!
* Added Potions:
  * Critical Potion
  * Berserk Potion
  * Potion of Iron Skin
  * Potion of Avarice
  * Gemini Potion
* Many more potions are to come! Some will even have a unique way of getting them. 
* Potions currently only have one level. Most of them will stay this way, but others will be getting more levels in the future.
</details>

<details>
   <summary> v1.4.1 </summary>

* Changed plasmaball counter to be global instead of per-orb. Now you can watch the countdown!
* Increased plasmaball line duration. It disappeared way too fast and made it almost invisible
* Buffed Berserkorb. Lvl 1: (1|1) -> (1|3) Lvl2: (1|1) -> (2|4) Lvl3: (1|1) -> (3|5)
* Buffed Orbgis. Lvl 1: (1|1) -> (1|2) Lvl2: (1|1) -> (2|3) Lvl3: (1|1) -> (3|4)
* Buffed lasorb. Lvl 3: (1|2) -> (2|3)
* Fixed lasorb continuous activation on long pegs
* Fixed the modification to Matryorshka and Sealed Convicition. They should now remove vanilla behavior in favor of modded instead of both at the same time. (This was fixed last patch, but forgot to put in patch notes)
</details>

<details>
   <summary> v1.4.0 </summary>

* Added new orb: Lasorb
* Added new orb: Orbgis
* Added new orb: Berserkorb
* Added new relic: Anvil
* Added new relic: Mystery Capsule
* Reverted Stone back to vanilla behavior
* Reverted Bouldorb back to vanilla behavior
* Reverted Orbelisk back to vanilla behavior

* Fixed invisible bombs having collision for predicitions
</details>

<details>
  <summary> v1.3.4 </summary>

* Updated dependancy Prolib.
* Changed speed-up as suggested by Imakunee
* Speed-up now works with vanilla speed-up.
* Speed-up now adjusts itself with the vanilla speed-up.
</details>

<details>
  <summary> v1.3.3 </summary>

* Updated to Peglin v.0.7.53
</details>

<details>
  <summary> v1.3.2 </summary>

* Updated dependancy ProLib
* Updated localization
* Fixed some orb descriptions
* Orbelisk has been buffed
</details>

<details>
  <summary> v1.3.1 </summary>

* Updated to v.0.7.48
* Minor prediction bug fixes
</details>

<details>
  <summary> v1.3.0 </summary>

* Split Promethium into two mods - one with content and the other as a library
* Updated to v0.7.45
* Buffed Holster
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
### Speed-Up
* A dynamic speed-up system that increases the gamespeed the longer your orb is in play.
* Highly configurable. You can change the delay, the rate of speed-up, and the maximum speed-up.
* Works together with vanilla speed-up.

### Retargeting
* You can now retarget after sending your orb out!

### Negative Effects
* All negative effects (i.e. Confusion) are removed while navigating.

### Dynamic Relic Icons
* You can now set relic icons to disappear if they are not relevant to what you are currently doing (I.e. relics that don't affect navigation won't appear while navigating). This is mainly aimed for cursed/endless runs, as your screen just becomes all relics. This feature requires you to have a minimum number of relics to activate, and can be turned off.

### Prediction System
* Prediction system is now much more accurate with pegnet and unicorn horn. There are still some edge cases, but generally should be accurate.

## New Mechanics
### Armor
Armor acts as a second resource for health. It is used up first, and has unique ways of replenishing it. The amount of armor you currently have is indicated by a status effect.

### Hold Ability
You can now hold an orb. To do so, use the discard action and it'll put your current orb in the hold slot. To discard, hold the discard button until the radial bar around the trash can fills up.

### Potions
Potions are a new type of orb. They act as consumables. Unless stated, they do not take up a turn when used. There are three types of potions:

#### Permanent Potions
These potions last for the remainder of the battle. They require an open potion slot to be used.

#### Durational Potions
These potions last for a set amount of turns before their effect runs out. They require an open potion slot to be used.

#### Instant Potions
These potions are used immediately and do not require an open potion slot.

### Curse
![Curse1](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_One.png)![Curse2](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Two.png)![Curse3](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Three.png)![Curse4](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Four.png)![Curse5](https://raw.githubusercontent.com/ruiner189/Promethium/main/Resources/Relics/Curse_Five.png)

Are you ready to test your pachinko skills? If you make it to the final boss, continuing the game will start a Cursed run. There are a total of 5 tiers. Can you make it to the end?
Each curse relic will make the enemies stronger in different ways. It'll also give the player a small buff so they might survive...

#### Starting a Curse Run
At the end of your current run (if you defeat the last boss), choose "Restart" to begin or increase your curse level. Each run will give you three choices of curse relics. The negative effect is the same, while you get to choose the benefit. 

Curse runs too easy? Check the configs to make them harder.

## New Relics
Excluding the curse relics, Promethium currently adds several new relics:
<details>
  <summary> Holster </summary>

  * Adds a new mechanic! You can now hold orbs alongside of discarding them.
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

<details>
   <summary> Anvil </summary>
   
   * Increases orb battle reward levels by 1
</details>

<details>
   <summary> Mystery Capsule </summary>

 * Give a random boss relic after hitting 500 pegs
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

<details>
<summary> Lasorb </summary>

Sends out a laserbeam between other lasorbs. Each beam will activate pegs in its path.

Level 1
* Attack: 1 | Crit: 2
* Hits all enemies in the same row as the targeted enemy
* Hits enemies 3 times each. Damage is split evenly per hit
* Sends a laser towards other lasorbs every 5 pegs hit
* Multiball 1
   
Level 2
* Attack: 1 | Crit: 2
* Hits all enemies in the same row as the targeted enemy
* Hits enemies 3 times each. Damage is split evenly per hit
* Sends a laser towards other lasorbs every 4 pegs hit
* Multiball 1
   
Level 3
* Attack: 2 | Crit: 3
* Hits all enemies in the same row as the targeted enemy
* Hits enemies 3 times each. Damage is split evenly per hit
* Sends a laser towards other lasorbs every 3 pegs hit
* Increases laser duration by 100%
* Multiball 1
</details>

<details>
<summary> Orbgis </summary>

Level 1
* Attack: 1 | Crit: 2
* Increases Maximum Armor by 2
* Start with +1 Armor each battle
* Gain +2 Armor when fired
   
Level 2
* Attack: 2 | Crit: 3
* Increases Maximum Armor by 4
* Start with +2 Armor each battle
* Gain +4 Armor when fired

Level 3
* Attack: 3 | Crit: 4
* Increases Maximum Armor by 6
* Start with +3 Armor each battle
* Gain +6 Armor when fired
</details>

<details>
<summary> Berserkorb </summary>

Level 1
* Attack: 1 | Crit: 3
* Overflow
* Attacks gain +1|+1 for every 20 missing health
* Multiplies damage based on current armor (x * 0.08)
* Removes 50% of current armor when fired

Level 2
* Attack: 2 | Crit: 4
* Overflow
* Attacks gain +1|+1 for every 15 missing health
* Multiplies damage based on current armor (x * 0.1)
* Removes 40% of current armor when fired

Level 3
* Attack: 3 | Crit: 5
* Overflow
* Attacks gain +1|+1 for every 10 missing health
* Multiplies damage based on current armor (x * 0.12)
* Removes 25% of current armor when fired
</details>

## Potions
<details>
<summary> Critical Potion </summary>

Level 1
* Every attack is a crit
* Lasts 3 turns
* Can only be used once per battle
</details>

<details>
<summary> Berserk Potion </summary>

Level 1
* Increases damage dealt by 1.5x
* Increases damage received by 2x
* Lasts 3 turns
* Can only be used once per battle
</details>

<details>
<summary> Potion of Iron Skin </summary>

Level 1
* Reduces damage received by 1
* Lasts 3 turns
* Can only be used once per battle
</details>

<details>
<summary> Potion of Avarice </summary>

Level 1
* Increases gold gained by 2x but deal 75% less damage
* Lasts 2 turns
* Can only be used once per battle
</details>

<details>
<summary> Gemini Potion </summary>

Level 1
* Clones the next two orbs two times and send them to the bottom of the deck. The clones do not last for the next shuffle.
* Can not clone other gemini potions
* Potion is activated immediately
* Can only be used once per battle
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
