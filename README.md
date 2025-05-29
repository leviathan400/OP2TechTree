# OP2TechTree

![Screenshot](https://images.outpostuniverse.org/OP2TechTree.png)

## What is it?

Outpost 2 tech tree editor. Open, edit or create new Outpost 2: Divided Destiny tech tree text files. 

Default tech trees are: edentek.txt, ply_tek.txt, tutortek.txt and multitek.txt


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
- `DESCRIPTION "string"` - Full research description (max 800 chars)
- `TEASER "string"` - Pre-research teaser text (max 800 chars)  
- `IMPROVE_DESC "string"` - Improvement description (max 800 chars)

### Requirements & Costs
- `REQUIRES int` - Required prerequisite technology ID
- `COST int` - Research cost for both colonies
- `EDEN_COST int` - Research cost for Eden colony
- `PLYMOUTH_COST int` - Research cost for Plymouth colony
- `MAX_SCIENTISTS int` - Maximum scientists (4-18, or 1 for special techs)
- `LAB int` - Required lab level (1=Basic, 2=Standard, 3=Advanced)

### Unit/Building Upgrades
- `UNIT_PROP token token int` - Unit property modification
  - Format: `UNIT_PROP [UNIT_TYPE] [PROPERTY] [VALUE]`

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
| 0 | Blank/No Icon |
| 1 | Basic |
| 2 | Defenses |
| 3 | Power |
| 4 | Vehicles |
| 5 | Factory |
| 6 | Food |
| 7 | Residence |
| 8 | Lab |
| 9 | Weapons |
| 10 | Rare |
| 11 | Morale |
| 12 | Research |

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
