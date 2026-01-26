# Guide: Manual Tilemap Painting & Layering

Since we disabled auto-generation, you can now paint your world by hand using Unity's Palette tools. This gives you full artistic control.

## 1. Setting up the Layers (Sorting Layers)
To draw "Grass on top of Water" or "Trees on top of Grass", we need **Sorting Layers**.

1.  Go to **Edit > Project Settings > Tags and Layers**.
2.  Open **Sorting Layers**.
3.  Add the following layers (drag to reorder them top-to-bottom):
    - **Default** (Bottom - for deep background)
    - **Water**
    - **Ground**
    - **Details** (Top - for rocks, flowers)
    - **Player** (So the player stands on top)

## 2. Setting up the Grid
1.  Select your `WorldGrid` (the parent object). 
2.  Ensure it has multiple Child Objects (Tilemaps). Rename them to match your structure:
    - `Tilemap_Water`: Set **Tilemap Renderer > Sorting Layer** to `Water`.
    - `Tilemap_Ground`: Set **Tilemap Renderer > Sorting Layer** to `Ground`.
    - `Tilemap_Details`: Set **Tilemap Renderer > Sorting Layer** to `Details`.

> **Tip**: Rotation X=90 is still needed on the Grid if you are doing a 3D/2.5D top-down view.

## 3. Using the Tile Palette
1.  Open **Window > 2D > Tile Palette**.
2.  Create a **New Palette** (e.g., "WorldPalette"). Save it in your Art folder.
3.  **Drag your Sprites/Rule Tiles** into this window.
    - Drag your `RuleTile_Grass`, `RuleTile_Water` asset files into the palette area.
4.  **Drawing**:
    - Select the **Brush Tool (B)**.
    - **Active Tilemap**: In the Palette window, look for the "Active Tilemap" dropdown.
    - Select `Tilemap_Water`. Paint your ocean.
    - Select `Tilemap_Ground`. Paint your islands on top.
    - Select `Tilemap_Details`. Paint decorations.

## 4. Layering Strategy (The "Sandwich")
- **Bottom (Water)**: Paint the entire map blue.
- **Middle (Ground)**: Paint your islands. The Rule Tiles will handle the edges.
- **Top (Hills/Details)**: Paint cliffs or hills on a higher Sorting Layer (or use the Z-height trick if using `Order in Layer`).

## 5. Saving
Your painted map is saved with the **Scene**. Just press Ctrl+S. The code will no longer overwrite it.
