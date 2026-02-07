# C# Coding Standards & Environment

## Async/Await

*   **UniTask**: Use `Cysharp.Threading.Tasks` (UniTask) for all asynchronous operations instead of standard `Task` or Coroutines.
    *   Example: `UniTask<WorldGrid>` instead of `Task<WorldGrid>`.
    *   Avoid `async void` except for specific Unity event handlers (and use `UniTaskVoid` where applicable).

## Code Structure

*   **File-scoped Namespaces**: Use file-scoped namespace declarations to reduce indentation.
    ```csharp
    namespace Game.Domain.Entities;
    // instead of namespace Game.Domain.Entities { ... }
    ```
*   **POCOs**: Keep Entities and UseCases as Plain Old C# Objects.

## Dependency Injection (VContainer)

*   **Constructor Injection**: All dependencies must be injected via constructor.
*   **No Service Locators**: Do not use `GameObject.Find`, `GetComponent` (outside of self), or Singleton `Instance` access.
*   **Registration**: Register all services in a `LifetimeScope`.

## Environment

*   **Telemetry**: Set `DISABLE_TELEMETRY=true` in the environment configuration to ensure that asset names and project structures are not transmitted externally.
