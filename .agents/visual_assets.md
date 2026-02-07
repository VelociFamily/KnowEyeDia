# Visual Asset Guidelines: 16x16 Pixel Art

## Asset Analysis

*   **Source**: "Puny World" style pixel art.
*   **Resolution**: 16x16 pixels per tile.
*   **Connectivity**: Full "Blob" tileset connectivity (47-tile bitmask) is implied by the presence of inner and outer corners in the source images.

## Rule Tile Configuration

*   **Neighbor Rule**: Use **3x3** check (not 2x2) to support complex corners.
*   **PPU**: All sprites must be imported with **Pixels Per Unit = 16**.
*   **Filter Mode**: **Point (No Filter)**.
*   **Compression**: **None**.

## Mapping Strategy

When generating the MapPresenter, map the logical `TileType` enums to specific `RuleTile` assets found in the project:

*   `TileType.DeepWater` -> `Assets/.../Water_RuleTile.asset`
*   `TileType.Grass` -> `Assets/.../Grass_RuleTile.asset`
*   `TileType.Wall` -> `Assets/.../Wall_RuleTile.asset`

> [!CRITICAL]
> Do not hardcode paths. Use `Resources.Load` or serialized fields in the `MapSettings` ScriptableObject.
