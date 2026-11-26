# Architecture Overview

How OP2TechTree is put together, for anyone (person or coding agent) about to change it.
Format follows https://architecture.md/. Keep it up to date as the code changes.

OP2TechTree is a Windows desktop editor for Outpost 2: Divided Destiny tech tree files
(`edentek.txt`, `ply_tek.txt`, `multitek.txt`, `tutortek.txt` and the map-specific `*tek.txt` files).
It opens, edits, validates and saves those text files, and renders them as an interactive HTML tech tree.

## 1. Project Structure

```
op2techtree/
|-- OP2TechTree.sln               # Visual Studio solution (one project)
|-- OP2TechTree/
|   |-- OP2TechTree.vbproj        # VB.NET WinForms, .NET Framework 4.8, OptionStrict Off
|   |-- fMain.vb                  # Main form: data model, file parse/save, editor UI, menu actions
|   |-- fMain.Designer.vb         # WinForms designer code for the main form (generated, edit via designer)
|   |-- TechTree.vb               # UI-free helpers: constants, validator, sample tree, CSV/stats, HTML exporter
|   |-- fAbout.vb / .Designer.vb  # About box
|   |-- My Project/               # AssemblyInfo (version), Resources, Settings
|   `-- Resources/
|       |-- techtree_template.html  # The browser tech tree page (embedded resource; data injected at export)
|       |-- icon1-13.bmp            # In-game research category icons (embedded into the HTML as PNG data URIs)
|       |-- Tech_Science.ico        # App icon
|       `-- banner05_620x100.png    # About box banner
|-- techs/                        # Published site: exported tree pages + source .txt files + index.html
|-- prototype/                    # Early standalone HTML prototype of the tree view (not used by the app)
|-- README.md                     # User guide and tech file format reference (Latin-1, CRLF)
|-- CHANGELOG.md                  # Release notes (new features only)
`-- ARCHITECTURE.md               # This document
```

Build output (Debug) goes outside the project, to `..\..\build\OP2TechTree.exe` (`SVN\opu\build`).

## 2. High-Level System Diagram

```
                +---------------------------- fMain (WinForms) -----------------------------+
 tech .txt  --> | LoadTechFile -> ParseLines/ApplyTag -> TechList + TechOrder + header/footer |
 (Latin-1 +     |                                         + checksum trailer bytes          |
  trailer)      |                                                 |                         |
                |   editor form <-> SaveCurrentTech/LoadTechToForm |                         |
                |                                                 v                         |
 tech .txt  <-- | SaveTechFile <- RenderFileText/EmitTech <- in-memory model               |
 (+ .bak)       |        |                    |                       |                     |
                +--------|--------------------|-----------------------|---------------------+
                         v                    v                       v
               TechValidator        TechImportExport          TechTreeHtmlExporter
               (errors list)        (CSV, statistics)         template + JSON data + icons
                                                                      |
                                                                      v
                                              *_techtree.html -> default web browser
                                              (%TEMP%\OP2TechTree\ for F5 view, or a chosen path for Export)
```

There is no server, database or network access. Everything happens on local files.

## 3. Core Components

### 3.1. Desktop editor (`fMain.vb`)

- **Data model** (nested classes, referenced elsewhere as `fMain.Technology` / `fMain.UnitProperty`):
  `Technology` holds ID, name, category, Eden/Plymouth cost, `UsesUnifiedCost`, scientists, lab,
  teaser/description/improve text, prerequisites, unit properties, plus `LeadingComments` and
  `ExtraLines` (unknown tags and in-tech comments) so they survive a save.
- **State:** `TechList` (ID -> tech), `TechOrder` (original file order), `HeaderLines`, `FooterLines`,
  `TrailerPrefix`/`TrailerBytes` (the stock OPU "; Garbage so checksum matches original:" tail),
  `CurrentTech`, `FilePath`, dirty flag.
- **Load:** reads raw bytes, splits off the checksum trailer, decodes Latin-1, parses into temporary
  collections and only swaps them in when the whole file parsed (a failed load changes nothing).
  `ReadQuotedString` handles multi-line quoted text.
- **Save:** makes `<file>.bak`, then `RenderFileText`/`EmitTech` write techs in `TechOrder` (new techs
  after, by ID), keeping comments, unknown tags and the trailer bytes. `COST` vs `EDEN_COST`/`PLYMOUTH_COST`
  is preserved per tech.
- **Editing:** the form edits one tech at a time; changes reach the model on Apply (`SaveCurrentTech`),
  and File > Save applies first. Renaming an ID updates every `REQUIRES`; deleting removes references.
- **Other views:** Dependencies tab (lazy-expanding requires / required-by trees), Find References,
  View Raw File, Statistics, Validate.

### 3.2. Helpers (`TechTree.vb`, no UI)

- `TechConstants`: categories, labs, unit types and properties, ID pattern, special cost values.
- `TechValidator`: per-tech and whole-tree checks (ID format, costs, scientists, missing prerequisites,
  cycles).
- `TechFileGenerator`: the sample tech tree.
- `TechImportExport`: CSV export and statistics.
- `TechTreeHtmlExporter`: loads the embedded `techtree_template.html`, serialises the techs to JSON
  (`JavaScriptSerializer`, with `</` escaped) and the category icons to PNG data URIs, and replaces
  `__TITLE__`, `__DATA__` and `__ICONS__`.

### 3.3. Tech tree page (`Resources/techtree_template.html`)

A single self-contained HTML page with inline CSS and JavaScript and no external requests. It lays
techs out left to right by prerequisite depth (campaign "Tech Level N" techs become mission bands),
draws SVG edges, highlights requires/unlocks on hover, shows details on click, and offers an
Eden/Plymouth cost toggle, search, drag-to-pan and themes (OP2 default, Auto, Light, Dark; stored in
`localStorage` key `op2techtree-theme`). The exported page is a snapshot of the in-memory tree,
including applied but unsaved edits.

### 3.4. Published site (`techs/`)

Exported pages for the stock OPU tech files, their source `.txt` files, a hand-written `index.html`
and a `README.md`. It shares the template's look and theme key but is not generated by the app.

## 4. Data Stores

- **Tech tree `.txt` files:** the only persistent data. Latin-1 text, tag-based
  (`BEGIN_TECH "Name" 05317` ... `END_TECH`), `;` comments, optional binary checksum trailer in the
  stock OPU files. Format reference: README.md.
- **`<file>.bak`:** previous version, written on every save.
- **Settings:** `My.Settings` (`OP2Path`, currently unused).
- **Temp:** `%TEMP%\OP2TechTree\<name>_techtree.html`, replaced on each F5.

## 5. External Integrations / APIs

None at runtime. The app opens HTML in the user's default browser via `Process.Start` (shell execute).

## 6. Deployment & Infrastructure

- Build: MSBuild / Visual Studio 2022 Build Tools, `OP2TechTree.sln`, Debug or Release. Requires
  .NET Framework 4.8 on the user's machine.
- Distribution: the `.exe` (resources are embedded). Source control is SVN (`SVN\opu`).
- Versioning: `fMain.ApplicationVersion` / `ApplicationBuild` and `My Project/AssemblyInfo.vb`
  (keep both in step). Release notes in CHANGELOG.md, features only.
- No CI, no automated packaging, no telemetry or logging beyond `Debug.WriteLine`.

## 7. Security Considerations

- Local, single-user tool; no authentication, network or secrets.
- HTML export: tech text goes into a `<script>` block as JSON with `</` escaped, so tech text cannot
  break out of the script. The page builds its cards and details panel with `innerHTML`, passing
  every tech field through `esc()`; keep doing that for any new field.
- Saving writes the file in place (`FileMode.Create`); the `.bak` copy is the recovery path.

## 8. Development & Testing Environment

- Open `OP2TechTree.sln` in Visual Studio 2022, or build from the command line with MSBuild.
- Test data: the stock files in `Outpost 2\OPU\base\techs` and `OPU\maps\*`.
- No automated tests. The main manual check is a round trip: open a stock file, save, save again and
  confirm the two saves are identical and every tech and comment is kept.
- `prototype/techtree_prototype.html` can be served with `python -m http.server` to iterate on the
  tree page outside the app; port the changes into `Resources/techtree_template.html`.

## 9. Future Considerations / Known Issues

Known parsing and editing risks not yet addressed:

- An empty cost box is saved as `0` (free / pre-researched).
- Inline comments after a value (`COST 1200 ; note`) are not stripped, so the value fails to parse.
- A `"` inside name or description text is written unescaped and breaks the file; an unclosed quote
  swallows the rest of the file on load.
- Duplicate tech IDs in a file are merged silently (last one wins), with no warning.
- The validator reports Eden and Plymouth cost both `-1` as an error, though stock files use it.
- The checksum trailer is kept but cannot match once the text changes.
- Unapplied form edits are lost without a prompt when switching techs.
- Saves are not atomic (no write-to-temp-then-rename).
- `OptionStrict Off` and broad `NoWarn` hide type errors; the data model lives inside the form class.

Possible larger changes: move parsing/saving out of `fMain` into a UI-free class so it can be
unit-tested; add a round-trip test over the stock tech files.

## 10. Project Identification

- Project Name: OP2TechTree
- Repository: SVN `opu/op2techtree`; website https://github.com/leviathan400/OP2TechTree
- Primary Contact: Outpost Universe (see About box)
- Date of Last Update: 2026-11-01 (version 0.7.0)

## 11. Glossary / Acronyms

- **OP2:** Outpost 2: Divided Destiny.
- **OPU:** Outpost Universe, the community that maintains the game's updated release and files.
- **Tech / tech tree file:** a research definition / the `.txt` file that lists them for a game mode or map.
- **Eden / Plymouth:** the two player factions; techs can have different costs for each.
- **Cost 0 / -1:** pre-researched (free) / disabled.
- **Tech Level N:** marker techs in the campaign files that gate research by mission.
- **Checksum trailer:** binary bytes at the end of stock OPU tech files, after the
  "; Garbage so checksum matches original:" comment.
- **Unified cost:** a tech written with a single `COST` tag instead of `EDEN_COST` and `PLYMOUTH_COST`.
