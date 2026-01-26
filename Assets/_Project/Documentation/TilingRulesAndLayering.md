# Guide: Tiling Rules & Layering Strategy

This guide explains how to set up the "Blob" ruleset (standard 47-tile or 15-tile set) seen in your asset pack to make edges look correct.

## 1. The "Blob" Concept
Your sprites likely follow a pattern where you have:
- A generic center block.
- Edges (Top, Bottom, Left, Right).
- Corners (Outer and Inner).

To make Unity "layer" them correctly (e.g., Grass "on top" of Water), you configure the **Rule Tile** to look for neighbors.

### The 3x3 Grid Key
- 🟩 (Green Arrow) = "This Tile" (Connect to same type)
- ❌ (Red X) = "Not This Tile" (Empty space or different biome)
- ⚪ (Empty) = Don't Care

## 2. Essential Rules (The 13-Rule Minimal Set)
For a standard RPG terrain, you need at least these rules. The order usually doesn't matter for the basic set, but Unity checks top-to-bottom.

### A. The Center (Full Block)
Usually the default sprite. If you add a rule for it:
| 🟩 | 🟩 | 🟩 |
| 🟩 | 👾 | 🟩 |
| 🟩 | 🟩 | 🟩 |
*Surrounded by itself.*

### B. The Edges (4 Rules)
**Top Edge** (Grass with air above)
| ❌ | ❌ | ❌ |
| 🟩 | 👾 | 🟩 |
| 🟩 | 🟩 | 🟩 |

**Bottom Edge**
| 🟩 | 🟩 | 🟩 |
| 🟩 | 👾 | 🟩 |
| ❌ | ❌ | ❌ |

**Left Edge**
| ❌ | 🟩 | 🟩 |
| ❌ | 👾 | 🟩 |
| ❌ | 🟩 | 🟩 |

**Right Edge**
| 🟩 | 🟩 | ❌ |
| 🟩 | 👾 | ❌ |
| 🟩 | 🟩 | ❌ |

### C. The Outer Corners (4 Rules)
**Top-Left Corner** (Island edge)
| ⚪ | ❌ | ❌ |
| ❌ | 👾 | 🟩 |
| ❌ | 🟩 | 🟩 |
*Note: Depending on your art, you might ignore the diagonal (top-left) or set it to X.*

**Top-Right Corner**
| ❌ | ❌ | ⚪ |
| 🟩 | 👾 | ❌ |
| 🟩 | 🟩 | ❌ |

**Bottom-Left Corner**
| ❌ | 🟩 | 🟩 |
| ❌ | 👾 | 🟩 |
| ⚪ | ❌ | ❌ |

**Bottom-Right Corner**
| 🟩 | 🟩 | ❌ |
| 🟩 | 👾 | ❌ |
| ❌ | ❌ | ⚪ |

### D. The Inner Corners (4 Rules)
These are for when you have land surrounding a hole or "L" shapes.
**Inner Top-Left** (Ground everywhere, but a diagonal bite taken out)
| 🟩 | 🟩 | 🟩 |
| 🟩 | 👾 | 🟩 |
| 🟩 | 🟩 | ❌ | 
*(Wait, this grid depends on which sprite represents the "Inner Corner". Usually, the sprite shows a corner of DIRT entering the WATER. If your sprite is "Grass surrounding a water dot", you check for the opposite.)*

**Standard Inner Corner Rule:**
Checking for the "Empty" diagonal.
Example: **Bottom-Right Inner Corner** of the sprite (visually).
| 🟩 | 🟩 | 🟩 |
| 🟩 | 👾 | 🟩 |
| 🟩 | 🟩 | ❌ |
*(Requires 'Not This Tile' in the diagonal spot, but 'This Tile' everywhere else)*.

## 3. Multiple Biomes (Layering)
How do you handle Dirt paths on top of Grass?

### Strategy A: Multiple Tilemaps (Recommended)
1.  **Background Tilemap**: Fill completely with "Grass".
2.  **Foreground Tilemap**: Paint "Dirt Path" on top.
    *   Set the `Renderer Sort Order` of Foreground to be higher (e.g., 1).
    *   This is the easiest way. The Dirt tiles don't need to know about Grass; they just treat "Empty" as their edge.

### Strategy B: "Not This" matching
If you put them on the *same* Tilemap:
1.  In the Dirt Rule Tile settings:
2.  Can you say "Connect to Grass"? No, standard Rule Tiles only do "This" or "Not This".
3.  So, Dirt will treat Grass as "Not Dirt" (Red X).
4.  This works fine for Islands (Dirt Island on Water).
5.  It looks BAD for patches (Dirt Patch inside Grass) unless your Dirt sprites have transparency that shows the grass underneath (rare in 16-bit).

**Best Practice**: Use **Strategy A**.
- `Layer_0_Ground`: Base terrain (Water/Void).
- `Layer_1_Terrain`: Main ground (Grass).
- `Layer_2_Details`: Paths (Dirt), Rocks.

This matches our `WorldView` setup (`_heightLayers`), although there we used it for vertical height. You can separate "Height" from "Texture Layering" if you want complex 2D layering, but for this project:
- **Grass** goes on `Layer_0`.
- **Hills** (2nd level) go on `Layer_1`.
