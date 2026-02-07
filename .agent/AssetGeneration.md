# Asset Generation Guide

This document defines the process for generating, cleaning, and integrating art assets for the "KnowEyeDia" project.

## 1. Style Guide
*   **Art Style:** Retro Pixel Art.
*   **Perspective:** Top-down or Pseudo-Isometric (2.5D) for world nodes.
*   **Dimensions:**
    *   **Items (Inventory/Hotbar):** 32x32 pixels (upscaled from 16x16 or native).
    *   **World Nodes:** 32x32 or 48x48 pixels.
    *   **Wiki/Marketing:** Upscale 400% or 800% (128px or 256px) using Nearest Neighbor.
*   **Background:** Transparent (preferred) or Solid White (#FFFFFF) for post-processing.

## 2. Tools
*   **Generation:** AI Image Generator.
*   **Cleaning:** `process_sprite.py` (Script in project root).
    *   *Usage:* `python process_sprite.py <path_to_image>`
    *   *Function:* Replaces white (and near-white) pixels with transparency.

## 3. Asset Categories & Naming Convention
Files should be saved in `Assets/Art/Sprites/`.

| Category | Subfolder | Naming | Example |
| :--- | :--- | :--- | :--- |
| **Items** | `Items/` | `Item_[Name].png` | `Item_IronOre.png`, `Item_IronIngot.png` |
| **Nodes (Finite)** | `Environment/` | `Node_[Name]_Finite.png` | `Node_Iron_Finite.png` |
| **Nodes (Infinite)** | `Environment/` | `Node_[Name]_Infinite.png` | `Node_Iron_Infinite.png` |

## 4. Workflow

### Step 1: Generate Image
Use the following prompt structure:
> "pixel art sprite of [OBJECT_NAME], white background, high contrast, retro game asset, 32-bit style"

### Step 2: Post-Process
Run the python script to remove the background:
```bash
python process_sprite.py "Assets/Art/Sprites/Items/Item_IronOre.png"
```

### Step 3: Unity Import Settings (Recommended)
*   **Texture Type:** Sprite (2D and UI)
*   **Sprite Mode:** Single
*   **Filter Mode:** Point (no filter)
*   **Compression:** None

## 5. Asset List

### Iron
*   `Item_IronOre.png`: Raw ore chunk.
*   `Item_IronIngot.png`: Refined bar.
*   `Node_Iron_Finite.png`: Rock with brown/rust spots.
*   `Node_Iron_Infinite.png`: Deep industrial drill hole or massive crystal outcrop.

### Copper
*   `Item_CopperOre.png`: Raw ore chunk, orange/green.
*   `Item_CopperIngot.png`: Refined bar.
*   `Node_Copper_Finite.png`: Rock with orange streaks.
*   `Node_Copper_Infinite.png`: Massive copper deposit.

### Coal
*   `Item_Coal.png`: Black lump.
*   `Node_Coal_Finite.png`: Dark rock with black patches.
*   `Node_Coal_Infinite.png`: Deep dark seam.

### Stone
*   `Item_Stone.png`: Grey pebble/rock.
*   `Node_Stone_Finite.png`: Standard grey boulder.
*   `Node_Stone_Infinite.png`: Quarry face.

### Wood
*   `Item_Wood.png`: Brown log.
*   `Node_Tree_Finite.png`: Standard Tree.
*   `Node_Tree_Infinite.png`: Ancient Mother Tree.
