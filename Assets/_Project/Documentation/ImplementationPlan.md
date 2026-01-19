# Random World Generation & Terrain Movement Plan

## Goal
To generate a random world using the `Pixel Crawler` asset pack and allow the player to move freely over the terrain with verticality.

## Asset Analysis
The `Pixel Crawler - Free Pack` contains:
- **Tilesets**: `Floors_Tiles.png`, `Dungeon_Tiles.png`.
  - *Note*: These spritesheets are currently sliced into large irregular chunks. For optimal tiling, they should be sliced into 16x16 or 32x32 tiles. We will use the available slices as placeholders or "chunks" for now, or use a specific slice if suitable.
- **Structures**: Buildings, Stations.
- **Entities**: Player, Enemies.

## User Review Required
> [!IMPORTANT]
> The asset pack's tilesets (`Floors_Tiles.png`) are not sliced into a standard grid (e.g., 16x16). The generation logic will use the available slices as "Map Chunks" or I will attempt to use a specific sub-sprite. **For best results, please re-slice the assets in the Unity Editor to a 16x16 grid.**

## Proposed Changes

### Domain Layer
#### [NEW] `WorldData.cs`
- POCO to hold map data: `TileType[,]`, `Height[,]`.

#### [NEW] `WorldGenerationUseCase.cs`
- logic to generate `WorldData` using Perlin Noise.
- Parameters: Seed, Width, Depth, Scale.

#### [NEW] `IWorldService.cs`
- Interface to access world data (e.g., `float GetHeightAt(float x, float z)`).

#### [MODIFY] `PlayerPresenter.cs`
- Inject `IWorldService` (or `WorldGenerationUseCase`).
- Update `Tick`:
    - Calculate `targetY` based on `GetMovementInput`.
    - Update `PlayerView` position including Y axis (Snap or Lerp to terrain height).

### Presentation Layer
#### [MODIFY] `WorldView.cs` (MonoBehaviour)
- **References**: `Tilemap` components (Ground, Hills, High), `TileBase` assets (Rule Tiles for Grass, Dirt, Stone).
- `Render(WorldData data)`: Sets tiles on the Tilemap `_tilemap.SetTile(pos, tileBase)`.
- **Verticality**: Use distinct Tilemaps for different elevations or "stack" tiles if logic allows. For simplicity in 2.5D:
    - Elevation 0: Ground Tilemap.
    - Elevation 1: Hills Tilemap (with collision).
    - Elevation 2: Mountain Tilemap.

#### [NEW] `WorldPresenter.cs`
- Orchestrates generation on Start.
- Maps `TileType` to specific `TileBase` assets (Rule Tiles) configured in the View or a ScriptableObject config.

#### [DELETE] `TilePrefab`
- Replaced by Unity `RuleTile` assets created in Editor.

### Tools / Editor
#### [NEW] `SpriteSlicerWindow.cs` (Editor)
- Custom Editor Window to assist with slicing.
- **Features**:
    - **Auto-Grid Detection**: Analyzes texture to suggest 16x16, 32x32, etc.
    - **Batch Slicing**: Apply slicing to multiple selected textures.
    - **Clean Slicing**: Option to ignore empty tiles.
- **Why**: Automates the manual task of slicing "Point Crawler" assets which are often packed irregularly.

### Infrastructure/Configuration
- Register new services/presenters in `GameLifetimeScope` (if exists) or Main Scope.

## Verification Plan
1.  **Play Mode Test**:
    - Run the scene.
    - Verify "World" is generated (grid of tiles).
    - Move Player.
    - Verify Player moves up/down visual hills (if height > 0).
