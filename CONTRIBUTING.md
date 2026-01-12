# Contributing

## Requirements
- Git 2.39+
- Git LFS installed and enabled
- Unity 6000.3.2f1 (see [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt))
- VS Code recommended (repo may track [.vscode](.vscode/))

## Git LFS
1. Install Git LFS (one-time per machine):
   ```powershell
   git lfs install
   ```
2. The repo tracks large binaries via [.gitattributes](.gitattributes). After cloning or switching branches, ensure LFS pointers are hydrated:
   ```powershell
   git lfs pull
   ```

## Unity Project Settings
- Version Control: Visible Meta Files
- Asset Serialization: Force Text
- Line Endings: Prefer LF

Meta files must be committed. Do not delete or ignore `*.meta`.

## Smart Merge (UnityYAMLMerge)
Configure globally (update the path to your Unity installation if needed):
```powershell
$unityYamlMerge = 'C:\Program Files\Unity\Hub\Editor\6000.3.2f1\Editor\Data\Tools\UnityYAMLMerge.exe'

git config --global merge.tool unityyamlmerge
git config --global mergetool.unityyamlmerge.cmd '"' + $unityYamlMerge + '" merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'
git config --global mergetool.unityyamlmerge.trustExitCode true
git config --global merge.unityyamlmerge.name 'Unity SmartMerge (YAML)'
git config --global merge.unityyamlmerge.driver '"' + $unityYamlMerge + '" merge -p %O %A %B %A'
```

## Ignored Files
Unity-generated and transient files are ignored via [.gitignore](.gitignore). Keep source, assets, and meta files tracked.

## Branches and PRs
- Create feature branches from `main`.
- Keep commits atomic; include tests under `Assets/Tests` where applicable.
- Validate play/build in Unity before opening a PR.
