# Simple Clean Architecture (SCA) Rules

The project follows a strict 5-layer architecture. Deviations are not permitted.

## 1. Entity Layer (Domain/Entities)

*   **Role**: Pure data structures (POCOs).
*   **Constraints**:
    *   NO using `UnityEngine` (except `Vector2Int` or basic structs).
    *   NO inheritance from `MonoBehaviour` or `ScriptableObject`.
    *   MUST be strictly serializable C# classes/structs (e.g., `WorldData`, `TileGrid`).

## 2. Usecase Layer (Domain/UseCases)

*   **Role**: Business logic and algorithms (e.g., Perlin Noise generation, Cellular Automata).
*   **Constraints**:
    *   NO reference to View, Presenter, or Unity UI.
    *   Input: `GenerationConfig` (Seed, Size).
    *   Output: `WorldData` (Entity).
    *   PURE C# logic only.

## 3. Gateway Layer (Infrastructure/Gateways)

*   **Role**: Interface adapters for external data (Save/Load, Network).
*   **Constraints**:
    *   Must implement interfaces defined in the Domain.

## 4. Presenter Layer (Presentation/Presenters)

*   **Role**: The glue between Logic and View.
*   **Constraints**:
    *   Entry Point for the scene.
    *   Orchestrates the call to `Usecase.Execute()`.
    *   Converts `WorldData` (int/enum) into `RuleTile` asset references.
    *   Passes renderable data to the View.

## 5. View Layer (Presentation/Views)

*   **Role**: Rendering and Input.
*   **Constraints**:
    *   Inherits `MonoBehaviour`.
    *   The ONLY layer allowed to reference `UnityEngine.Tilemaps`.
    *   **Passive View** pattern: Does not decide *what* to draw, only *how*.
    *   MUST use bulk update methods (`SetTiles`) for performance.
