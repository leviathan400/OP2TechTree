# OP2TechTree

![Screenshot](https://images.outpostuniverse.org/OP2TechTree.png)

## What is it?

Outpost 2 tech tree editor. Open, edit or create new Outpost 2: Divided Destiny tech tree text files. 

Default tech trees are: **edentek.txt**, **ply_tek.txt**, **tutortek.txt** and **multitek.txt**

The techs are stored in `\OPU\base\techs`.

The demo techs are stored in `\OPU\maps\OfficialDemos`.

### Tech Tree File Types

- **edentek.txt** - Eden single-player campaign technology tree
- **ply_tek.txt** - Plymouth single-player campaign technology tree  
- **tutortek.txt** - Tutorial mission's technology tree
- **multitek.txt** - Multiplayer technology tree (both factions)

Each file defines different technology progressions, costs, and availability for different game modes and factions.

## Features

- Open, edit and save tech tree `.txt` files, with search and category filtering
- Edit costs per faction, prerequisites, unit property upgrades, lab level and scientists
- Changing a Tech ID updates every `REQUIRES` that references it; deleting a tech removes those references
- Validate a single tech or the whole tree (missing prerequisites, circular dependencies, limits)
- Statistics, dependency view and "Find References" for any tech
- Export to CSV
- Civilization-style tech tree view in your web browser, with HTML export
- Saving keeps the original file layout: comments, tech order, unknown tags (`PLAYER_PROP`, `FUNCTION_RESULT`, ...) and the checksum trailer in the stock OPU files. A `.bak` copy of the previous file is made on every save

## Tech Tree View

**Tools > View Tech Tree in Browser** (F5) shows the whole tech tree Civilization-style in your default web browser.

- Techs are arranged left to right: each tech sits one column to the right of its deepest prerequisite
- In the campaign files (`edentek.txt`, `ply_tek.txt`) the "Tech Level N" techs become mission bands across the top
- Cards show the in-game category icon, research cost, lab level, scientists and number of unit upgrades. Pre-researched techs (cost 0) are green and disabled techs (cost -1) are greyed out
- Hover a tech to highlight everything it requires (amber) and everything it leads to (blue)
- Click a tech for its full details: costs, prerequisites, unit upgrades, teaser and description
- Switch between Eden and Plymouth costs (shown when a file has faction-specific costs), search by name or ID, and drag to pan
- Theme button: OP2 (the default, styled after the in-game command panel), Auto (follows your system light/dark setting), Light or Dark. The choice is remembered by your browser

The view shows the tech tree as it is in the editor, including changes applied but not yet saved. Press F5 again after editing to refresh it. The page is written to `%TEMP%\OP2TechTree\` and replaced each time.

**Tools > Export Tech Tree to HTML...** saves a copy you can keep next to your `.txt` file (the save dialog opens in that file's folder). The HTML file is self-contained, with no internet connection or OP2TechTree needed to open it. It is a snapshot, so export again after changing the tech tree.

# Outpost 2 Tech Tree File Format

## Overview

Outpost 2 tech tree files define the research technologies available to players, including their costs, requirements, descriptions, and unit/building upgrades. The tech files use a simple tag-based format with specific syntax rules and limitations.

## File Structure

Each technology is defined between `BEGIN_TECH` and `END_TECH` tags:

```
BEGIN_TECH "Technology Name" TECHID
    [TAGS...]
END_TECH
```

## Tech Tags Reference

### Required Tags
- `BEGIN_TECH` - Opens technology definition with name and ID
- `END_TECH` - Closes technology definition

### Basic Information Tags
- `CATEGORY int` - Technology category/icon (0-12)
- `DESCRIPTION string` - Full research description (max 800 chars)
- `TEASER string` - Pre-research teaser text (max 800 chars)  
- `IMPROVE_DESC string` - Improvement description (max 800 chars)

### Requirements & Costs
- `REQUIRES int` - Required prerequisite technology ID (can have multiple)
- `COST int` - Research cost for both colonies
- `EDEN_COST int` - Research cost for Eden colony (-1 = disabled, 0 = free)
- `PLYMOUTH_COST int` - Research cost for Plymouth colony (-1 = disabled, 0 = free)
- `MAX_SCIENTISTS int` - Maximum scientists (4-18, or 1 for special techs)
- `LAB int` - Required lab level (1=Basic, 2=Standard, 3=Advanced)

### Unit/Building Upgrades
- `UNIT_PROP token token int` - Unit property modification
  - Format: `UNIT_PROP [UNIT_TYPE] [PROPERTY] [VALUE]`

## Faction-Specific Technology System

### Cost / Research Time System
- **COST** alone: Same cost for both factions
- **EDEN_COST / PLYMOUTH_COST**: Faction-specific costs

| Value | Effect | Visibility | Notes |
|-------|--------|------------|-------|
| `COST 0` | Pre-researched | Invisible in lab menu | Already completed |
| `COST -1` | Disabled | Not available | Faction restrictions |
| `COST > 0` | Available | Shows in lab or completed list | Research time (higher = longer) |

Cost values represent research time, not game resources. Higher values take longer to research.

### Asymmetric Faction Design
The cost system enables asymmetric faction gameplay:
- **Eden** starts with Laser weapons (free) but cannot research Microwave weapons (disabled)
- **Plymouth** starts with Microwave weapons (free) but cannot research Laser weapons (disabled)

## Technology Progression

### Campaign Structure
Single-player campaigns use **Tech Levels** (01000, 02000, 03000, etc.) to gate technology progression by mission. Each mission unlocks specific tech levels, making certain technologies available for research.

### Prerequisites
Technologies can require multiple prerequisites using multiple `REQUIRES` tags. All prerequisites must be completed before a technology becomes available for research.

## System Limits

### Global Limits
- **Maximum Technologies**: 256 total techs per file
- **Tech Name Length**: 100 characters maximum
- **Description Length**: 800 characters maximum (DESCRIPTION, TEASER, IMPROVE_DESC)
- **Number Buffer**: 32 bytes maximum, must fit in 32-bit signed integer

### Per-Technology Limits  
- **REQUIRED_TECH Tags**: 32 maximum per tech
- **Upgrade Tags**: 32 maximum per tech (UNIT_PROP, PLAYER_PROP, FUNCTION_RESULT combined)

### Validation Rules
- Tech Level = Tech ID ÷ 1000, must not be 0
- Tech ID should not be negative
- Armor property must be 0-5 (inclusive)
- MAX_SCIENTISTS: 4-18 (normal techs) or 1 (special/free techs)
- LAB values: 1, 2, or 3 only
- COST values: -1 (disabled), 0 (free), or positive integers
- Use either COST alone, or both EDEN_COST and PLYMOUTH_COST

## Valid Unit Types

```
ACID, AGRIDOME, ASE, BIG_TANK, BIG_TRUCK, BULL_DOZER, CANNON, COMMAND,
CON_TRUCK, DIRT, EMP, EMP_MISSILE, FACT_ANDROID, FACT_LUXURY, 
FACT_STRUCTURE, FACT_VEHICLE, FOAM, FORUM, GARAGE, GEO_METRO, 
GEOTHERMAL, GORF, LAB, LAB_ADV, LAB_STANDARD, LASER, LIGHT_TANK, 
MAGMA_WELL, MED_CENTER, MED_TANK, METEOR_DEF, MICROWAVE, MINE, 
MINE_ADV, MINER, MOBILE_HOME, NURSERY, OBSERVATORY, RAIL_GUN, 
RECREATION, REPAIR_TRUCK, RESIDENCE, RESIDENCE_E, RESIDENCE_P, 
ROBOT_COMMAND, SCORPION, SCORPION_WEAPON, SCOUT, SELF_DESTRUCT, 
SELF_DESTRUCT_ADV, SMELTER, SMELTER_ADV, SOLAR, SPAM, SPACEPORT, 
SPIDER, STORAGE_ORE, STORAGE_RARE, SURVEYOR, THORS_HAMMER, TOKAMAK, 
TOWER_GUARD, TOWER_LIGHT, TRADE, TUBE, UNIVERSITY, WALL_TRUCK
```

## Valid Unit Properties (UNIT_PROP)

| Property | Usage | Applied To |
|----------|-------|------------|
| `Hit_Points` | Unit health/durability | Units, generally vehicles |
| `Armor` | Damage resistance (0-5) | GuardTower, Lynx, Panther |
| `Production_Capacity` | Build rates & capacity | Buildings, factories |
| `Move_Speed` | Movement speed | Vehicles |
| `Sight_Range` | Vision range | LightTower, GuardTower, Weapons |
| `Rate_Of_Fire` | Weapon firing rate | Weapons |
| `Build_Time` | Construction time | *Not used in standard files* |
| `Power_Required` | Power consumption | GORF, Smelter, Rare Smelter |
| `Storage_Capacity` | Storage/training capacity | University train rate |
| `Storage_Bays` | Storage bays | *Not used in standard files* |
| `Production_Rate` | Production efficiency | Colony building, repair vehicles |
| `Concussion_Damage` | Area damage | Weapons |
| `Penetration_Damage` | Direct damage | Weapons |
| `Improved` | Unit improvement flag | Mine, RareMine, Smelter, RareSmelter, GORF, Observatory |
| `Workers_Required` | Worker requirements | DIRT |
| `Common_Required` | Common ore cost | Units, generally vehicles |
| `Rare_Required` | Rare ore cost | Units, generally vehicles |
| `Turn_Rate` | Turning speed | *Not used in standard files* |
| `Build_Points` | Build point cost | *Not used in standard files* |

## Categories (Icons)

| Value | Category |
|-------|----------|
| 0 | No Icon / Blank / Free |
| 1 | Lab Icon / Database Recovery |
| 2 | Factory Icon / Infrastructure |
| 3 | Power Icon / Power Generation |
| 4 | Vehicle Icon / Vehicles & Robotics |
| 5 | Food Icon / Food Production |
| 6 | Mining Icon / Mining & Metals |
| 7 | Weapons Icon / Weapons |
| 8 | Spaceport Icon / Space Program |
| 9 | Morale Icon / Population |
| 10 | Geological Icon / Disasters |
| 11 | Emergency Icon / Medical & Emergency |
| 12 | Starship Icon / Starship Components |

### Category Usage
- **Category 0**: Used for free/unlocked technologies and tech level markers
- **Category 1**: Database Recovery technologies appear primarily in single-player campaigns (rebuilding lost Earth knowledge)
- **Categories 2-12**: Research technologies organized by functional area

## Example Technology
```
BEGIN_TECH "Reinforced Vehicle Construction" 05317
    CATEGORY        4
    DESCRIPTION     "ConVec, Cargo Truck, and Evacuation Transport Hit Points increased."
    TEASER          "Increases ConVec, Cargo Truck, and Evacuation Transport Hit Points."
    IMPROVE_DESC    "Increased hit points"
    REQUIRES        05110
    EDEN_COST       1200
    PLYMOUTH_COST   1400
    MAX_SCIENTISTS  12
    LAB             2
    UNIT_PROP CON_TRUCK Hit_Points 375
    UNIT_PROP CON_TRUCK Common_Required 1000
    UNIT_PROP BIG_TRUCK Hit_Points 750
    UNIT_PROP BIG_TRUCK Common_Required 500
    UNIT_PROP MOBILE_HOME Hit_Points 280
    UNIT_PROP MOBILE_HOME Common_Required 650
END_TECH
```

## Important Notes

- **String Formatting**: Quoted strings have embedded newline and tab characters stripped
- **Tag Overwriting**: Single-value tags overwrite previous values if used multiple times (excludes REQUIRES, UNIT_PROP, PLAYER_PROP, FUNCTION_RESULT)
- **Numbers**: Can have optional leading minus sign (no positive sign allowed)
- **Pairing**: Every BEGIN_TECH must be paired with END_TECH
- **Conventions**: Most techs specify at least CATEGORY and COST
- **Lab Requirements**: Lab value typically not set for free and disabled techs

## Design Philosophy

The Outpost 2 tech tree system supports:
- **Asymmetric faction gameplay** through differential costs and availability
- **Mission-gated progression** in single-player campaigns
- **Branching research paths** with multiple prerequisites
- **Meaningful choices** between different strategic directions
- **Unit progression** through upgrade technologies that improve existing units
