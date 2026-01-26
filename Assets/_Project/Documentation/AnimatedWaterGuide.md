# Guide: Animated Rule Tiles (Water & Islands)

Since your `Water_Tiles` contain animation frames (for the island edges and the water waves), we need to configure the Rule Tile to cycle through these sprites instead of showing a static image.

## 1. Preparation
Ensure your `Water_Tiles` are sliced.
*   **Island Row**: 4 Frames (e.g., `Water_0`, `Water_1`, `Water_2`, `Water_3`).
*   **Water Type A**: 2 Frames.
*   **Water Type B**: 2 Frames.

## 2. Creating the Animated Rule Tile
1.  Create a new Rule Tile: **Create > 2D > Tiles > Rule Tile**. Name it `RuleTile_Water`.
2.  Select it to open the Inspector.

## 3. Configuring an Animated Rule
For each rule (Center, Edge, etc.) where you want movement:

1.  **Add a new Rule** (+).
2.  **Set the Rules**: Set the Green Arrows/Red Xs as usual (e.g., for the generic ocean, maybe no rules or just "Is Water").
3.  **Change Output to Animation**:
    *   Look at the **Output** column (far right of the rule row).
    *   Change it from **Single** to **Animation**.
4.  **Assign Frames**:
    *   The property list below will change. Set **Size** to the number of frames (e.g., `4` for the island, or `2` for the water).
    *   Drag your sliced sprites (`Water_0`, `Water_1`...) into the **Element** slots.
5.  **Adjust Speed**:
    *   Set **Speed** to something like `0.2` or `0.5` (Lower is slower).
    *   *Tip*: Ensure "randomize" is off if you want them to loop synchronously, or on if you want noisy water logic.

### Example: The "Deep Ocean" (Default)
If you want the default "fill" to be animated water:
1.  Go to the **Default Sprite** section at the top.
2.  Unfortunately, the **Default Sprite** is usually static in the basic Inspector.
3.  **Workaround**: create a "Don't Care" rule at the very bottom of the list.
    *   **Conditions**: All Empty (Don't Care).
    *   **Output**: Animation.
    *   **Sprites**: Drag your 2 Water Frames here.
    *   This ensures that any tile that doesn't match an edge will turn into animating water.

### Example: The Island Edge
1.  Add Rule: Green Arrow (This) on Bottom, Red X (Not This) on Top.
2.  Output: **Animation**.
3.  Sprites: Drag the 4 Island Frames.
4.  Speed: Match the water speed.

## 4. Troubleshooting
*   **"TileAutomatorship"**: My previous script couldn't handle this because it assumes 1 static sprite per position. Manual setup is best here creates the highest quality result.
*   **Speed Mismatch**: If the island animates faster than the water, adjust the **Speed** value on that specific rule.
