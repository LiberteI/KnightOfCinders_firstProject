# Known Issues and Testing

## Known Limitations
This repository preserves the gameplay code, architecture, and main scenes, but a few practical limitations remain:

1. The repository is not fully reproducible as a clean original development environment. Gameplay code and main scenes remain, but some original Unity project context and setup metadata were lost after an accidental deletion.
2. Testing is manual only. There are no automated unit, integration, or regression tests.
3. The architecture is modular by feature, but many systems still rely on serialized references and direct component access rather than stricter interface boundaries.
4. Some in-level camera transitions still need tuning, especially the dungeon-to-exploration handoff.

## Testing Approach
Testing for this project was entirely manual.

Core validation areas:
- player movement and responsiveness
- combo flow and stamina gating
- hit detection and combat feedback
- boss phases and encounter progression
- camera transitions and arena lock-in
- ambience, UI, and cutscene flow

Manual testing method:
- repeated in-editor playtesting
- tuning based on combat feel and pacing
- repeated encounter runs to validate trigger, cleanup, and progression flow

## Reproducibility Notes
- The codebase remains useful for review of gameplay architecture and systems implementation.
- A surviving demo executable also exists, which helps preserve the playable result even though the original environment is incomplete.

## What I Would Improve Next
- Add automated tests for high-risk gameplay rules and state transitions
- Improve camera-transition polish in narrow traversal spaces
- Reduce direct component coupling with better interfaces and namespaces
- Add recorded GIFs and short system demos tied directly to the docs

## Employer-Relevant Takeaway
Even with the repository limitations, the project still provides strong evidence of:
- solo gameplay systems implementation
- debugging through repeated playtesting
- iteration on encounter flow and combat feel
- maintaining a non-trivial multi-system Unity project
