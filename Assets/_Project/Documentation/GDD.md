# Game Design Document (GDD)
**Project Title:** Untitled Retro Survival Automation
**Genre:** 2.5D Survival / Factory Automation / Sandbox
**Visual Style:** SNES-era Pixel Art Sprites (2D) inside a modern 3D Environment (Billboarding).

## 1. High Concept
A survivor crashes on a procedural alien planet. They must manage survival vitals while mining resources to build automated machines. The ultimate goal is to research technology, automate production, and construct a spaceship to escape.

## 2. Visuals & Camera
**Perspective:** 2.5D. The environment is 3D (terrain, water, depth). Characters, trees, machines, and items are 2D pixel art sprites (Billboards) that always face the camera or have 4-directional sprites.
**Camera:** Isometric or Top-Down angled.
**Art Direction:** SNES palette, vibrant but dangerous.

## 3. Core Gameplay Loop
1.  **Survive:** Maintain Health, Hunger, and Temperature.
2.  **Gather:** Mine ores and chop trees (manual at first).
3.  **Process:** Build machines to automate refining (Ore -> Ingot).
4.  **Research:** Unlock new tech (3x3 grid -> 4x4 grid -> Space Parts).
5.  **Escape:** Build the Ark Ship.

## 4. Key Mechanics

### A. Vitals
**Health:** Reaches 0 = Death.
**Hunger:** Decreases over time. Low hunger damages Health.
**Temperature:** Environmental threat. Need fire/shelter in cold biomes, cooling in hot biomes.

### B. Death & Respawn
**Condition:** On death, the player drops a "Corpse Container" containing all inventory items.
**Respawn:** Player spawns at world start or set bed. Must perform a "Corpse Run" to retrieve items.

### C. Inventory & Crafting
**Structure:** Minecraft-style Hotbar (0-9) + Main Inventory Grid.
**Crafting Progression:**
    * The player has an intrinsic crafting grid inside their inventory.
    * **Tier 1:** 3x3 Grid.
    * **Tier 2:** Upgrade to 4x4 Grid.
    * **Tier 3:** Upgrade to 5x5 Grid.
**Automation:** Placeable machines (Drills, Belts, Smelters) handle resources in the world.

### D. Procedural World
**Generation:** Seed-based procedural terrain.
**Parameters:**
    * Water Scarcity (Slider).
    * Forest Density (Slider).
    * Ore Density (Slider, scales up to 1,000,000+ for late game "infinite" nodes).

## 5. Controls (Input System)
**Support:** Mouse & Keyboard (KBM) and Gamepad.
**Scheme:**
    * **Move:** WASD / Left Stick.
    * **Interact:** E / South Button (Open chests, machines).
    * **Use Item:** Left Click / West Button.
    * **Inventory:** Tab / Start.
    * **Pause:** Esc.