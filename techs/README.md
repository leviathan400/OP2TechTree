# Outpost 2 Tech Trees

Interactive research trees for **Outpost 2: Divided Destiny**, built from the game's tech files
(the Outpost Universe / OPU release). Open **[index.html](index.html)** and pick a tree.

Each page shows every tech in the file, what it requires and what it unlocks. Pages are
self-contained HTML: no install, no internet connection needed, and they work straight from a
folder on your computer as well as online.

## Using a tree page

- Techs run left to right: each tech sits one column to the right of its deepest prerequisite.
  In the campaign trees the "Tech Level N" techs become mission bands across the top.
- Cards show the category icon, research cost, lab level, scientists and number of unit upgrades.
  **Green** = pre-researched (cost 0), **grey** = disabled (cost -1).
- **Hover** a tech to highlight everything it requires (amber) and everything it leads to (blue).
- **Click** a tech for its full details: costs, prerequisites, unit upgrades, teaser and description.
- **Eden / Plymouth** switches between the two factions' costs.
- **Search** by tech name or ID; **drag** to pan around the tree.
- **Theme** button: OP2 (the default, styled after the in-game command panel), Auto (follows your
  system light/dark setting), Light or Dark. Your browser remembers the choice.

## The trees

| Tree | Source file | Used by | Techs |
|---|---|---|---|
| **Base game** (`OPU/base/techs`) | | | |
| Eden | `edentek.txt` | Eden campaign | 135 |
| Plymouth | `ply_tek.txt` | Plymouth campaign | 137 |
| Multiplayer | `multitek.txt` | Multiplayer (both factions) | 137 |
| Tutorial | `tutortek.txt` | Tutorial | 95 |
| **Mission maps** (`OPU/maps`) | | | |
| Survival (170) | `survtech.txt` | Caught in the Crossfire 2, Fractured Alliance | 170 |
| Survival (168) | `survtech168.txt` | Darkest Hour, Judgment Day, Research and Development, Yukon Trail | 168 |
| Survival (163) | `survtech165.txt` | Caught in the Crossfire, Forsaken World | 163 |
| Exiles (co-op) | `cooptech.txt` | Exiles | 154 |
| Danger Zone | `flsurvtek.txt` | Danger Zone | 152 |
| Pursued | `PursuedTechTree.txt` | Pursued | 145 |
| To the Skies or to the Grave | `surv_tek.txt` | To the Skies or to the Grave | 144 |
| Multiplayer, no lava walls | `multitekNoLavaWalls.txt` | Rising from the Ashes | 137 |
| Swarm | `swarmtek.txt` | Swarm | 46 |
| Scout Rush | `scouttek.txt` | Scout Rush | 24 |
| Bomber's Big Blast | `blasttek.txt` | Bomber's Big Blast | 3 |
| **Official demos** (`OPU/maps/OfficialDemos`) | | | |
| Demo, multiplayer | `demotekm.txt` | Official demos | 110 |
| Demo M | `dtek_m.txt` | Official demos | 105 |
| Demo P | `dtek_p.txt` | Official demos | 102 |
| Demo E | `dtek_e.txt` | Official demos | 89 |

Tech counts include the "Tech Level" marker techs. Some files have entries commented out with `;`,
which the game and the pages leave out: Pursued has Evacuation Transport (03607), and the older
survtech has two "Project Tankn00b" techs (12000, 12001), so its 165 entries give 163 techs.

### The three survtech.txt versions

OPU ships three different files all named `survtech.txt`, each in its map's own folder. Here the
older two are renamed so they can sit side by side; the files themselves are unchanged:

- `survtech.txt`: 170 techs, the newest (Caught in the Crossfire 2, Fractured Alliance)
- `survtech168.txt`: 168 techs (Darkest Hour, Judgment Day, Research and Development, Yukon Trail)
- `survtech165.txt`: 165 entries, 163 techs (Caught in the Crossfire, Forsaken World)

The maps sharing a version use byte-identical copies of the file. To use a renamed file in the
game, name it back to `survtech.txt`.

## Files

- `index.html`: the list of trees
- `<name>_techtree.html`: one page per tree
- `<name>.txt`: the original tech file each page was built from, unchanged

SHA-256 of the source files, to check they match your own install:

| File | SHA-256 |
|---|---|
| `PursuedTechTree.txt` | `0b8013451f588d5a5c27eecc3f8e198da671bb7c1a6d11783c22fd4ffe79c852` |
| `blasttek.txt` | `da1f71b640ecdf5cf1e0dae77aba44af4b07aee3d047795eb8dbc6814e1a0b4b` |
| `cooptech.txt` | `8098bb13a11de9346206ba3b8e2b0af7832f9261f6e0730d73772b740f38a1ed` |
| `demotekm.txt` | `e6c470469fef86781f7ae6e5b9df8296ab1951287e1800d049dfc73118245a5d` |
| `dtek_e.txt` | `9a759a5171bfa5eddbf436eedea7ed2f6113db28303110d57c408bbc4d74f58c` |
| `dtek_m.txt` | `5e781e4d0dd4448613a73152361d1778e9576315c04766a867791a789787aacc` |
| `dtek_p.txt` | `5fbacd6b00d27798a4f4e562eb3a63894fd3f72e7bd551a1b469cbe39ce33382` |
| `edentek.txt` | `a238d4d458beb873a4398a93af8481ae608a866105da4eb5ec1ad2feb3debe64` |
| `flsurvtek.txt` | `d42012acb7728978b56139228e8056231fb4f5449f382f9ccb0dd2cca5b3b28f` |
| `multitek.txt` | `6911698f0cbf311719c7ec4bc0dd072b72a76c71e20f7bc8eda3fd497a58641b` |
| `multitekNoLavaWalls.txt` | `3fc598fb0bfe88aedcf267e67ae5ebc0b99aa55ea26e6709bae19394b7615df9` |
| `ply_tek.txt` | `f0c4112acfce225b117af17b6b60f355cc80b156fac82f402482467a95a5b59b` |
| `scouttek.txt` | `ac5cbf2a5f4f31f9c07981bc9d4479ada53c43851dc9adc58b172fb61b69faa4` |
| `surv_tek.txt` | `8b9cfa9d64a17f1f40bece9f9ebd9f85d8f44a5fe90bb472d43e4beb6dcbe434` |
| `survtech.txt` | `eeef83883a2e25c1ae5233d00d4e368004ffdcf085803e89181fd89ade5d320a` |
| `survtech165.txt` | `403b4f514ce768ba68eed3b08d058a87990bfb3174420f10fb0b29330f14b7ec` |
| `survtech168.txt` | `59f5f7ce68761f91060d6f92304da710b1d8b5275ff38e4870578f9827edc390` |
| `swarmtek.txt` | `6f9bc75dce3e493bdbd315f28265fa6ab4fe67d632f0d474844d408c9df84f99` |
| `tutortek.txt` | `ac7462e0faf88731cb127b12caa813789f005594d6e83429d0e4b0d1c682c836` |

## How it was made

The pages were exported with **OP2TechTree**, the Outpost 2 tech tree editor
(Tools > Export Tech Tree to HTML). Each page is a snapshot of its `.txt` file.

Outpost 2 is © Sierra / Dynamix. The OPU tech files come from the Outpost Universe community release.
