# Technical Specifications
**Framework:** Unity 6
**Architecture:** Simple Clean Architecture (SCA) + VContainer
**Async:** UniTask
**UI:** UI Toolkit (MVVM)

## 1. Domain Layer (Entities)
Pure C# Classes. No Unity dependencies.

**PlayerEntity:**
    * Properties: CurrentHealth, MaxHealth, Hunger, BodyTemp.
    * Components: InventoryComponent (holds ItemData).
**ItemData:**
    * ID, Name, StackSize, SpriteReference.
**InventoryEntity:**
    * Logic for slots, stacking, and splitting.
    * Special Logic: CraftingGridSize (int) - changes from 3 to 5.
**WorldConfig:**
    * Struct holding generation seeds and density sliders (OreDensity float).

## 2. UseCase Layer (Game Logic)
Pure C# Classes. Executes rules.

**PlayerSurvivalUseCase:**
    * Method: Tick(float deltaTime) - reduces hunger, calculates temp damage.
**CraftingUseCase:**
    * Input: Recipe, InputItems.
    * Output: ResultItem.
    * Validation: Checks if recipe fits in current CraftingGridSize.
**WorldGenerationUseCase:**
    * Logic: Uses Noise maps (Perlin/Simplex) to generate ChunkData.
    * Note: Handle "Infinite Sources" by treating Ore Density > 1000 as a special "Rich Node" type.

## 3. Infrastructure/Gateways
Interface Adapters.

**IInputService:**
    * Wraps Unity's InputSystem. Exposes IObservable<Vector2> OnMove.
**ISaveSystem:**
    * Uses JsonSerializer.
    * Methods: SaveGame(string slotName), LoadGame(string slotName), AutoSave().
**IAssetProvider:**
    * Addressables or Resources wrapper to load Sprites for UI/World.

## 4. Presentation Layer (Unity Specific)

### View (Monobehaviour/UI Toolkit)
**PlayerView:**
    * SpriteRenderer (Billboard script: transform.rotation = Camera.main.transform.rotation).
    * Animator.
**HUDView (UI Document):**
    * Binds to HUDViewModel.
    * Displays Health/Hunger bars and Hotbar.
**InventoryView (UI Document):**
    * Dynamic Visual Tree generation based on CraftingGridSize (3x3 vs 5x5).

### Presenter
**PlayerPresenter:**
    * Subscribes to IInputService.
    * Calls PlayerMovementUseCase.
    * Updates PlayerView position.

## 5. Multiplayer Strategy (Preparation)
**Strict Separation:** Logic (UseCases) must be deterministic where possible.
**State:** All game state is held in Entities, not in Monobehaviours. This allows us to sync Entity data across the network later without rewriting the renderer.

## 6. Directory Map
Scripts/Domain/Entities/
Scripts/Domain/UseCases/
Scripts/Infrastructure/Gateways/
Scripts/Presentation/Presenters/
Scripts/Presentation/Views/
Scripts/Installers/ (GameLifetimeScope.cs)