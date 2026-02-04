# Git LFS Troubleshooting & Best Practices

This document outlines how to resolve issues with large files (over 100MB) causing push failures on GitHub, specifically within the KnowEyeDia Unity project.

## The Problem
GitHub has a hard limit of **100MB** for files stored in regular Git. If a file (like a large `.unity` scene or a `.prefab`) exceeds this limit and is not tracked by Git LFS (Large File Storage), your `git push` will fail.

## How to Fix Push Failures
If you have already committed a large file and your push is failing, follow these steps:

### 1. Migrate the History
Run this command to detect files matching the extensions and move them into LFS tracking, rewriting your local history:
```bash
git lfs migrate import --include="*.unity,*.prefab,*.asset"
```
*Note: This only affects your local branch history. If you have already pushed these commits (unlikely if it's failing), you would need to force push.*

### 2. Verify Tracking
Check that LFS is now tracking the files:
```bash
git lfs ls-files
```

### 3. Push to GitHub
Now that the files are in LFS, the push should succeed:
```bash
git push
```

---

## Best Practices for New Large Files

### 1. Ensure LFS is Installed
Run this once on any new developer machine:
```bash
git lfs install
```

### 2. Track File Types Early
If you are about to add a new type of large file (e.g., a 4K texture or a long audio track), ensure it is tracked:
```bash
git lfs track "*.xxx"
```
This updates the `.gitattributes` file, which **must** be committed.

### 3. Repository Guidelines
The following extensions should ALWAYS be in LFS for this project:
- Textures: `*.png`, `*.jpg`, `*.psd`, `*.tga`
- Audio: `*.wav`, `*.mp3`, `*.ogg`
- 3D Models: `*.fbx`, `*.obj`
- Large Unity Assets: `*.unity`, `*.prefab`, `*.asset` (if they grow large)
- Video: `*.mp4`, `*.mov`
