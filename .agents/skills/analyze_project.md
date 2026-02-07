---
name: analyze_project
description: Instructions for analyzing the Unity project state using MCP tools.
---

# Analyze Project Skill

Perform the following steps sequentially to understand the Unity project's procedural generation context.

## 1. Asset Discovery (`list_assets`)

*   **Goal**: Find all `RuleTile` assets to be used in generation.
*   **Instruction**: Run `list_assets` filtering for `t:RuleTile`.
*   **Analysis**: Check filenames for keywords: "Ground", "Water", "Grass", "Wall". Do not hallucinate paths; use only what is returned.

## 2. Scene Inspection (`manage_scene`)

*   **Goal**: Ensure the correct hierarchy exists for `Tilemap` generation.
*   **Instruction**: Inspect the active scene for a hierarchy: `Grid -> Tilemap_Ground -> Tilemap_Walls`.
*   **Action**: If missing, note that you must create a `Grid` with Cell Size `(1, 1, 0)` later.

## 3. Layer Analysis (`execute_script`)

*   **Goal**: Verify required Sorting Layers exist.
*   **Instruction**: Use `execute_script` to run the following snippet:
    ```csharp
    Debug.Log(string.Join(", ", SortingLayer.layers.Select(l => l.name)));
    ```
*   **Verification**: Check for layers named "Water", "Ground", and "Collision". If missing, include their creation in your implementation plan.

## 4. Asset Verification

*   **Goal**: Confirm `RuleTile` configuration.
*   **Instruction**: If you cannot inspect the internal YAML of the `RuleTile`s to verify sprites, assume they are correct. However, plan to create a `MapAssetRegistry` ScriptableObject to manually link logical `TileId` enums to these physical assets.
