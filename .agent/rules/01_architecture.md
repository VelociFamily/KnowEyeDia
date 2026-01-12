---
trigger: always_on
---

Project Architecture: Simple Clean Architecture (SCA)
This project strictly follows Simple Clean Architecture. You must categorize all new logic into one of five layers. Do not create generic "Managers" or "Controllers."

The 5 Layers (Strict Dependency Rules)
Entity (Domain/Entities)

Role: Pure data and business rules (e.g., PlayerStats, Weapon).

Constraint: Pure C# Class. NO MonoBehaviour. NO UnityEngine namespace (unless absolutely necessary for types like Vector3).

Dependencies: None.

Usecase (Domain/UseCases)

Role: Specific game logic execution (e.g., TakeDamageUseCase, EquipItemUseCase).

Constraint: Pure C# Class. NO MonoBehaviour.

Dependencies: Can depend on Entity and Gateway (Interfaces only).

Gateway (Infrastructure/Gateways)

Role: Interface adapters for data/external systems (e.g., PlayerPrefsRepository, CloudSaveSystem).

Constraint: Implementation of Domain interfaces.

Dependencies: Can use Unity APIs (e.g., PlayerPrefs).

Presenter (Presentation/.../Presenters)

Role: The "Glue" between logic and view. Handles input and updates the View.

Constraint: Can be MonoBehaviour or Pure C# (entry point).

Dependencies: Depends on Usecase and View.

Pattern: Use UniTask for async operations. Use Reactive properties (UniRx/Observables) to drive the View.

View (Presentation/.../Views)

Role: Rendering and Input capture only.

Constraint: MonoBehaviour or UI Document. "Passive View" pattern - it does not decide when to show things, only how.

Dependencies: None. It exposes methods/events for the Presenter.

Dependency Injection (VContainer)
Never use GameObject.Find, GetComponent (outside of local scope), or Singletons (Instance).

All dependencies must be injected via Constructor Injection [Inject] or the Configure() method in a LifetimeScope.

Every scene must have a LifetimeScope that acts as the Composition Root.

# Architecture Rules: Simple Clean Architecture (SCA)

You are an expert Unity Systems Architect. You must enforce **Simple Clean Architecture (SCA)** and **VContainer** dependency injection.

## Core Dependency Rules
1.  **Strict Layering**: Code must reside in one of five layers. Dependencies only point **inward**.
    *   **Entity** (`Domain/Entities`): Pure C# data/logic. NO `UnityEngine` namespace. NO dependencies on outer layers.
    *   **Usecase** (`Domain/UseCases`): Pure C# game logic. Implements `IUseCase`. Depends only on `Entity` and `Gateway` interfaces.
    *   **Gateway** (`Infrastructure/Gateways`): Data access/External APIs. Implements Domain interfaces.
    *   **Presenter** (`Presentation/.../Presenters`): Glues Logic to View. Depends on `Usecase` and `View`.
    *   **View** (`Presentation/.../Views`): UI/Rendering only. `MonoBehaviour` or `UI Document`. Passive.

## Dependency Injection (VContainer)
*   **NO Singletons**: Do not use `public static Instance`. Do not use `Singleton<T>`.
*   **NO Service Locators**: Do not use `GameObject.Find`, `FindObjectOfType`, or `GetComponent` (except for getting components on the *same* GameObject in a View).
*   **Injection**: All dependencies must be injected via **Constructor Injection** (`[Inject] public Class(IDependency dep)`).
*   **Scopes**: Register all services in a `LifetimeScope`.

## Code Constraints
*   **Entities & Usecases**: Must be POCOs (Plain Old C# Objects). Never inherit `MonoBehaviour`.
*   **Views**: Must implement an interface (e.g., `IPlayerView`) if accessed by a Presenter.