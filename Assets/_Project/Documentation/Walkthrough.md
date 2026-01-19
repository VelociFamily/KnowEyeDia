# Walkthrough: Random World Generation & Terrain Movement

I have implemented a random world generation system using Perlin Noise and Unity Tilemaps with Rule Tiles. I also included a custom **Sprite Slicer Tool**.

## 1. Setup Assets (First Time)
1.  **Slice Sprites**: 
    - Use **Tools > Sprite Slicer**.
    - Slice `Floors_Tiles.png` into 16x16.
2.  **Create Rule Tiles**:
    - Follow the [Rule Tile Guide](RuleTileGuide.md) to create `RuleTile_Grass`, `RuleTile_Dirt`, etc.

## 2. Setup Scene (Tilemaps)
1.  **Create Grid**:
    - Right-click Hierarchy > **2D Object > Tilemap > Rectangular**. This creates a `Grid` with a child `Tilemap`.
    - Rename `Grid` to `WorldGrid`.
    - **Crucial**: Set `WorldGrid` (or the Tilemaps) Rotation X to **90**. This makes the map lie flat on the ground (XZ plane).
2.  **Create Layers**:
    - Rename the default `Tilemap` to `Layer_0`.
    - Duplicate it to create `Layer_1` and `Layer_2`.
    - set `Layer_1` Position Y to **1** (or 0.5 depending on your visual scale).
    - set `Layer_2` Position Y to **2**.
    - *Tip*: Enabling "Sort Order" or "Order in Layer" usually applies to 2D view. For 3D, ensure the Y position is actually different.
3.  **Setup WorldView**:
    - Create a GameObject `World` (or use the one from before).
    - Add `WorldView` component.
    - **Configuration**:
        - Drag `Layer_0`, `Layer_1`, `Layer_2` into the **Height Layers** list.
    - **Rule Tiles**:
        - Assign your `RuleTile_Grass` assets to the corresponding slots (Grass, Dirt, etc.).
4.  **Update LifetimeScope**:
    - Ensure `GameLifetimeScope` references the `World` object.

## 3. Play & Verify
1.  Press **Play**.
2.  You should see the Rule Tiles filling the grid.
    - The edges should look nice (auto-tiled) if you set up the rules.
    - Hills should appear as stacked layers.
3.  **Move Player**:
    - The player should snap up/down when walking onto higher layers.

## Troubleshooting
- **Tiles standing up?** You forgot to rotate the Grid/Tilemap X=90.
- **Player floating?** Check `PlayerPresenter` height logic vs the visual Y position of your layers.
- **Missing Edges?** Check your Rule Tile configuration.
