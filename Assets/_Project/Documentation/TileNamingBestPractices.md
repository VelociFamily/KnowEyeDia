# Tile Naming Best Practices for Procedural Generation

## Objective
To establish a consistent, human-readable naming convention for sprite tiles that simplifies:
1.  **Rule Tile Creation**: Easily finding the correct sprite for each neighbor rule.
2.  **Debugging**: Quickly identifying missing or incorrect tiles.
3.  **Automation**: Enabling potential scripts to auto-assign tiles based on names.

## The Naming Convention
We usage a **Hierarchical Snake Case** format:
`[Biome]_[Type]_[Position]_[Variant/Frame]`

### 1. Biome (Required)
The general theme or material of the tile.
*   `Grass`
*   `Dirt`
*   `Water`
*   `Stone`
*   `Sand`

### 2. Type (Optional)
Sub-function of the tile, if it's not the base ground.
*   `Wall`
*   `Floor` (default, often omitted for base terrain)
*   `Deco` (flowers, rocks)
*   `Cliff`
*   `Edge`

### 3. Position (Required for Rule Tiles)
Describes where this tile fits in the 3x3 grid relative to the "mass" of the terrain.
*   **Corners**: `TopLeft`, `TopRight`, `BotLeft`, `BotRight`, `InnerTopLeft`, `InnerTopRight`, `InnerBotLeft`, `InnerBotRight`.
*   **Edges**: `Top`, `Bottom`, `Left`, `Right`.
*   **Center**: `Center` (The filled middle tile).
*   **Isolated**: `Single` (Surrounded by nothing).

**Standard Rule Tile Set (Full 47-tile support or simplified 16-tile):**
*   `Grass_TopLeft`
*   `Grass_Top`
*   `Grass_TopRight`
*   `Grass_Left`
*   `Grass_Center`
*   `Grass_Right`
*   `Grass_BotLeft`
*   `Grass_Bottom`
*   `Grass_BotRight`
*   `Grass_InnerTopLeft` (Concave corner)

### 4. Variant/Frame (Optional)
For animated tiles or random variations.
*   `_01`, `_02`, `_03` (Random noise)
*   `_Frame0`, `_Frame1` (Animation)

## Examples

| Old Name (Generic) | New Name (Structured) | Description |
| :--- | :--- | :--- |
| `Floors_Tiles_0` | `Grass_TopLeft` | Top-left corner of a grass island. |
| `Floors_Tiles_1` | `Grass_Top` | Top edge of grass. |
| `Floors_Tiles_5` | `Grass_Center_01` | Filled grass tile (Variation 1). |
| `Water_tiles_0` | `Water_Deep_Center_Frame0` | Animated deep water, first frame. |

## Workflow with Smart Sprite Renamer
1.  **Isolate the Group**: Focus on one Biome at a time (e.g., just the Grass block).
2.  **Set the Base Name**: Input `Grass` or `Grass_Green`.
3.  **Walk the Grid**: Use the tool to rename in order of the standard 3x3 grid (TL, T, TR, L, C, R, BL, B, BR).
4.  **Save**: Ensure you verify the preview before committing.

## Why This Matters for Rule Tiles
When configuring a Rule Tile, you will look for "Top Left" sprites. If your file is named `Grass_TopLeft`, the search is instant. If it's named `Floors_Tiles_42`, you have to squint at the thumbnail.
