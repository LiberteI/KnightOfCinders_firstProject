# Knight of Cinders

A solo-built 2D Unity action game focused on melee combat, boss phases, and finite-state-machine-driven enemy behavior.

## Recruiter Quick Scan
- Solo gameplay programming project
- Built in about 2 months
- Estimated effort: ~600 hours
- Unity / C#
- 5 FSM-driven actor families: player, tutorial skeleton, skeleton squad, Dark Wolf, Evil Wizard phase 1 / phase 2
- 4 major encounter types: Trap Skeleton, Skeleton Army, Dark Wolf, Evil Wizard
- Major systems: melee combat, stamina/health, event-driven hit pipeline, squad-role AI, boss phase logic, camera/scene flow, cutscenes, ambience/UI

## Project Summary
Knight of Cinders is a 2D Souls-like action game where the player fights through a ruined kingdom to defeat an evil wizard and reclaim the realm. The project emphasizes deliberate melee combat, readable telegraphs, stamina management, and escalating encounter design.

The codebase is structured around actor-specific finite state machines, manager-based gameplay components, and an event-driven combat pipeline. The strongest implementation areas are combat flow, enemy behavior, boss phase logic, and encounter orchestration.

## Download
- Demo build: [Google Drive download](https://drive.google.com/file/d/1QigchoK-Ckn5wDokIMy8jG526GJFgCHP/view?usp=sharing)
- Trailer: [YouTube trailer](https://www.youtube.com/watch?v=DgtHopB85VI)

## Controls
- `WASD` move
- `J` light attack
- `U` heavy attack
- `K` jump
- `L` roll
- `H` shield strike
- `Left Shift` sprint
- `Esc` quit

## Technical Highlights
- Component-based Unity gameplay architecture using focused `MonoBehaviour` managers
- Actor-specific finite state machines for player, enemies, and bosses
- Event-driven combat pipeline built around `HitData` and a shared `EventManager`
- Skeleton squad role system with `Frontliner`, `Flanker`, and `Backuper` behavior switching
- Multi-phase boss logic for Dark Wolf and Evil Wizard
- Stamina, health, hit stop, camera shake, and combat-audio feedback systems
- Arena lifecycle orchestration with camera lock-in, barriers, activation, and cleanup
- Scene-context switching, ambience control, and cutscene handoff

## What This Demonstrates
- Gameplay programming in Unity with C#
- Object-oriented system decomposition across combat, movement, AI, UI, and presentation
- Finite-state-machine architecture for multiple actor families
- Event-driven runtime design
- Encounter and boss design implementation
- Debugging and manual playtesting discipline

## Key Implementation Challenges
- Building reusable FSM patterns across player, enemies, and multi-phase bosses
- Keeping melee combat readable while supporting combos, directional blocking, rolling, and stamina costs
- Coordinating multi-enemy tactical behavior in the skeleton encounter
- Managing boss fight lifecycle across triggers, cameras, barriers, ambience, and cleanup
- Connecting gameplay outcomes to cutscenes and scene handoff without tightly coupling systems

## Technical Deep Dive
- [Architecture](docs/ARCHITECTURE.md)
- [Combat System](docs/COMBAT_SYSTEM.md)
- [Enemy AI and Bosses](docs/ENEMY_AI_AND_BOSSES.md)
- [Scene Flow and Presentation](docs/SCENE_FLOW_AND_PRESENTATION.md)
- [Known Issues and Testing](docs/KNOWN_ISSUES_AND_TESTING.md)
- [Game Design](docs/GAME_DESIGN.md)

## Known Limitations
- The repository preserves the gameplay code and main scenes, but the original full Unity project context was not fully retained after an accidental deletion.
- Testing is manual only; there are no automated gameplay or regression tests.
- Some systems are modular by responsibility but still rely heavily on serialized references and direct component access.
- A few in-level camera transitions still need tuning, especially around dungeon/exploration handoff.
