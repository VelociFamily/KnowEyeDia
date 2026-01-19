# Guide: Setting Up Rule Tiles

Rule Tiles are a powerful Unity feature that automatically selects the correct sprite based on the neighboring tiles. This is perfect for making terrain (grass, dirt) that connects seamlessly.

## Prerequisites
- **Sprites Sliced**: Ensure your spritesheet is sliced into individual 16x16 tiles (use the *Sprite Slicer Tool* we built).
- **Package Installed**: Ensure `2D Tilemap Extras` is installed (it is in your project manifest).

## Step 1: Create a Rule Tile
1.  In the **Project** window, right-click in your desired folder (e.g., `Assets/Art/Tiles`).
2.  Select **Create > 2D > Tiles > Rule Tile**.
3.  Name it (e.g., `RuleTile_Grass`).

## Step 2: Configure the Default Sprite
1.  Select the new `RuleTile_Grass`.
2.  In the **Inspector**, find the **Default Sprite** slot.
3.  Drag the "Center" or "Full" block sprite here. This is what shows up in the palette icon or if no rules match.

## Step 3: Define Rules based on Neighbors
You need to add a "Tiling Rule" for each edge case (Corners, Edges, Inner Corners).

1.  Click the **+ (Plus)** button to add a new Rule.
2.  **Assign Sprite**: Drag the specific sprite for this rule (e.g., Top-Left Corner) into the Sprite slot.
3.  **Set Constraints** (The 3x3 Grid):
    - **Green Arrow / Check**: Requires a neighbor of *this same tile type*.
    - **Red X**: Requires *empty space* (or a different tile).
    - **Empty**: Cop doesn't care (any neighbor).

### Common Rule Patterns (The "Blob" Set)
Imagine the center square is YOUR tile.

**1. Top-Left Corner** (Grass with air above and to left)
| | | |
|:---:|:---:|:---:|
| **X** | **X** | |
| **X** | **O** | **->** |
| | **->** | **->** |
*Set the top and left neighbors to **Red X**. Set Right and Bottom to **Green Arrow**.*

**2. Top Edge** (Grass with air above)
| | | |
|:---:|:---:|:---:|
| **X** | **X** | **X** |
| **->** | **O** | **->** |
| **->** | **->** | **->** |
*Set Top row to **Red X**. Set Left/Right/Bottom to **Green Arrow**.*

**3. Inner Corner** (Grass surrounded, but one corner is missing)
*Complex rules often required for deep inner corners.*

> **Tip**: If you are overwhelmed by the manual setup, search for "Unity Rule Tile Templates" or use the "Automated" setting if your asset pack adheres to a standard 47-tile layout.

## Step 4: Using It
1.  Once configured, you can paint with this single `RuleTile_Grass` item in the Tile Palette.
2.  Assign this `RuleTile` asset to our `WorldView` script (which we are about to update).
