# Procedural Generation Walkthrough

I have implemented a procedural map generation system using Perlin Noise with biome support, satisfying the requirements for 2x2 tile blocks and specific layering.

## Changes

1.  **Scene Creation**: Created `Assets/Scenes/ProceduralScene.unity` (copied from SampleScene).
2.  **Domain Logic**:
    *   Updated `WorldData` to include `Desert` and `Snow` tile types.
    *   Refactored `WorldGenerationUseCase` to generate maps using Height and Temperature noise.
    *   Enforced 2x2 tile block generation to avoid "weird straight edges" on single tiles.
    *   Implemented Biome scaling (features ~32 tiles large).
3.  **Visualization**:
    *   Updated `WorldView` to support distinct Tilemaps for each biome (Snow, Grass, Stone, Desert, Dirt, Water).
    *   Ensured Water is handled correctly (bottom layer, effectively).
4.  **Integration**:
    *   Updated `WorldPresenter` to run generation on Start with a 128x128 map.
    *   Created `MapSceneFixer` (Editor Script) to automatically assign the correct Tilemaps and Tile Assets in the new scene.

## How to Test

1.  Open `Assets/Scenes/ProceduralScene.unity`.
    *   *Note: Upon opening, the `MapSceneFixer` script should automatically run and link the Tilemaps and Tile Assets to the `WorldView` component on the "World Grid" object.*
2.  Press **Play**.
3.  A 128x128 map should generate with:
    *   Water bodies (Lakes/Oceans).
    *   Biomes: Desert, Grass, Snow based on "Temperature".
    *   Mountains (Stone) at high elevations.
    *   All tiles in 2x2 blocks.

## Configuration

You can tweak the generation parameters in `WorldPresenter.cs`:
*   `scale`: Controls the zoom of the noise (smaller number = larger biomes, I tuned it to `0.02f` for the 128 size to give large ~32+ tile features).
*   `width/depth`: Map size.
*   `seed`: Currently random, but can be fixed.
