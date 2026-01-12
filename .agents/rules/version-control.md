# Version Control Rules

- Use Git LFS for large binaries (textures, audio, video, models). Patterns are defined in [.gitattributes](../../.gitattributes).
- Commit all `*.meta` files and YAML assets (`*.unity`, `*.prefab`, etc.).
- Keep Unity in Force Text serialization and Visible Meta Files.
- Do not commit `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`, `UserSettings/`, or IDE caches (`.vs/`, `.idea/`, `.Rider/`).
- Prefer LF line endings; text normalization is set in [.gitattributes](../../.gitattributes).
- Configure UnityYAMLMerge globally; merges for YAML are mapped via `.gitattributes`.
- Before pushing, run:
  ```powershell
  git lfs pull
  git status
  ```
