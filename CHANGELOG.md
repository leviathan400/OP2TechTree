# Changelog

All notable changes to OP2TechTree are listed here, newest first.

## [0.8.0] - 2026-11-01

### Added
- **Themes for the tech tree view and HTML exports**: a theme button in the page header switches between OP2, Auto (follows the system light/dark setting), Light and Dark
  - OP2 theme (the default) styled after the in-game command panel: black panels, phosphor-green text and frames, silver window bar and red ASSIGN / STOP style buttons
- **Close Tech Tree**: File > Close Tech Tree (Ctrl+W) closes the open tech tree, offering to save unsaved changes first

### Fixed
- An empty cost box saved as 0, making the tech free / pre-researched. Empty or invalid costs are now rejected
- Inline comments (`COST 1200 ; note`) broke the value they followed. They are now read as comments and kept
- A double quote typed into a name or description broke the saved file; it is now rejected. An unclosed quote in a file no longer swallows the rest of the file, and problems found while loading are reported

## [0.7.0] - 2026-11-01

### Added
- **Tech tree view**: Tools > View Tech Tree in Browser (F5) shows the whole tech tree Civilization-style in your default web browser
  - Each tech sits one column to the right of its deepest prerequisite; in the campaign files the "Tech Level N" techs become mission bands
  - Cards show the in-game category icon, cost, lab level, scientists and unit upgrade count; pre-researched techs are green, disabled techs greyed out
  - Hover a tech to highlight everything it requires and everything it leads to; click for full details
  - Eden / Plymouth cost toggle (for files with faction-specific costs), search, drag to pan
  - Shows the tech tree as it is in the editor, including applied changes that are not saved yet
- **Export Tech Tree to HTML**: Tools > Export Tech Tree to HTML... saves a self-contained copy to keep next to your `.txt` file
- **Dependencies tab**: shown for techs that have prerequisites or are required by other techs
  - "Requires" and "Required by" trees that expand to follow the full chain, with direct and full-chain counts
  - Double-click a tech to open it; missing prerequisites and circular dependencies are flagged in red

### Fixed
- Saving added blank lines between techs and at the end of the file on every save, so files grew each time
- Opening a file, or creating the sample tech tree, kept the previously selected tech in the editor; saving could then copy that tech into the newly opened file
- Unknown tags (`PLAYER_PROP`, `FUNCTION_RESULT`, ...) were deleted on save, and comments inside a tech moved to the next tech. Both are now kept with their tech
- A file that failed to load left a half-cleared tech tree that still pointed at the previous file, so a save could overwrite that file with partial data. Loading now changes nothing unless the whole file reads successfully

## [0.6.0] - 2026-05-26

- Small updates

## [0.5.0] - 2025-05-30

- Category icons added to the display

## [0.4.0] - 2025-05-29

- First version of the Outpost 2 tech tree editor
