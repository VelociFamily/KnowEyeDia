# Documentation Policy

## Rule: Save Guides to Project Documentation
Any time you create a User Guide, Manual, Walkthrough, or significant Documentation Artifact (e.g., .md files explaining workflows, tools, or systems), you **MUST** save a copy to the project's documentation folder:

`[ProjectRoot]/Assets/_Project/Documentation/`

### Reasoning
This ensures that documentation is version-controlled alongside the assets and code, making it accessible to the user across different machines and not just locked within the Agent's temporary memory or artifacts folder.

### Implementation
1.  Generate the artifact as usual (for the user to review).
2.  Once approved (or concurrently), write the file to `Assets/_Project/Documentation/[Filename].md`.
3.  Ensure the filename is descriptive.
