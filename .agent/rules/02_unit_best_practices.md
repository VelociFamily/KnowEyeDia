Unity Engineering Standards (2025/2026)
1. UI Development (UI Toolkit)
Do not use UGUI (Canvas) for standard interfaces. Use UI Toolkit.

MVVM Pattern: All UI must use the Model-View-ViewModel pattern.

UXML: Layout definition.   

USS: Styling (use variables for colors/fonts).   

ViewModel: Pure C# class holding the state (use NotifyPropertyChanged).

View: MonoBehavior that binds the UXML to the ViewModel.

Runtime Binding: Use Unity 6 Runtime Data Binding where possible to link UXML properties to ViewModel C# properties.

2. Asynchronous Logic
Do not use Coroutines (IEnumerator / StartCoroutine).

Use UniTask: Use async UniTask and async UniTaskVoid for all asynchronous operations.

cancellation: Always pass a CancellationToken to async methods to handle object destruction safely.

3. Code Style & Performance
File Scoped Namespaces: Use namespace MyNamespace; to reduce indentation.

formatting: K&R style brackets.

Loops: Avoid LINQ in Update() loops. Avoid new allocations in Update().

Strings: Use ZString or pre-allocated builders for frequent string concatenations.