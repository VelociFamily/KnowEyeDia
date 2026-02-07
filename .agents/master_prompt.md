Subject: Procedural Map Generation System Implementation (SCA compliant)

Context:
I have initialized the `.agents` folder with `architecture.md` (SCA), `visual_assets.md` (16x16 PPU rules), and `unity_standards.md`.

Task:
Orchestrate the creation of a procedural generation system using the `unityMCP` server to analyze my project state and generate the necessary C# code.

Execution Steps:

1. **Discovery & Analysis (MCP)**
    *   Connect to the `unityMCP` server.
    *   Run `list_assets` to find all assets of type `RuleTile`.
    *   Analyze the filenames. Look for terms like "Ground", "Water", "Grass", "Wall".
    *   Run a script to list all `SortingLayer`s. Verify if layers named "Water", "Ground", and "Collision" exist. If not, note this for the setup plan.

2. **Asset Verification**
    *   Confirm that the found `RuleTile` assets are configured. If you cannot read the internal YAML detailing the sprites, ASSUME they are correctly set up but create a `MapAssetRegistry` ScriptableObject that allows me to manually link the Logical `TileId` to the Physical `RuleTile`.

3. **Architectural Implementation (SCA)**
    *   Generate `WorldGrid.cs` (Entity) using a flat 1D array for memory optimization.
    *   Generate `GenerateTerrainUseCase.cs` (Usecase). Implement a **Perlin Noise** algorithm for the base terrain. Add a generic `Seed` parameter. Ensure it returns a `UniTask<WorldGrid>`.
    *   Generate `MapPresenter.cs`. This must be the entry point (implement `IStartable` from VContainer). It should request the map generation and pass the result to the View.
    *   Generate `TilemapView.cs` (View). It must reference a `Tilemap` component. **CRITICAL**: Use `Tilemap.SetTiles(positions, tiles)` for the render pass. Do NOT use `SetTile` inside a loop.

4. **Dependency Injection**
    *   Create or Update `GameLifetimeScope.cs`. Register the Usecase (Scoped), the View (Component), and the Presenter (EntryPoint).

5. **Validation**
    *   Create a small Editor Script `DebugMapGen.cs` that allows me to click a button in the Inspector to trigger a generation pass for testing without playing the game.

Constraints:
*   Strictly follow the 16x16 pixel density context.
*   Ensure all async code uses `UniTask` and passes `CancellationToken`.
*   Do not guess asset names; use the ones you found in Step 1.
